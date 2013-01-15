using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Security;

namespace MobitelAPISender
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback((rcvSender, pCertificate, pChain, pSSLPolicyErrors) => true));

            string username;
            string password;
            List<string> numbers = new List<string>();
            string message;

            string outStr = "Unknown error.";

            string[] lines = System.IO.File.ReadAllLines("mapi_in.txt", Encoding.UTF8);

            if (lines.Length >= 5)
            {
                username = lines[0];
                password = lines[1];
                int i = 2;
                for (i = 2; i < lines.Length; i++)
                {
                    if (lines[i] == "---")
                    {
                        break;
                    }
                    numbers.Add(lines[i]);
                }
                if (i == lines.Length - 1)
                {
                    message = "";
                }
                else
                {
                    i++;
                    message = "";
                    for (; i < lines.Length; i++)
                    {
                        message += lines[i];
                        if (i < lines.Length - 1)
                            message += "\n";
                    }
                }

                if (numbers.Count > 0)
                {
                    try
                    {
                        if (MobiRemoteApi.MobiService.SendSMS(username, password, numbers.ToArray(), message))
                            outStr = "OK";
                        else
                            outStr = "Error when sending.";
                    }
                    catch (Exception e)
                    {
                        outStr = "API error: " + e.Message;
                    }
                }
                else
                {
                    outStr = "Need at least one number.";
                }
            }
            else
            {
                outStr = "Not enough lines.";
            }
            System.IO.File.WriteAllLines("mapi_out.txt", new string[] { outStr });
        }
    }
}

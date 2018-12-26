using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace Keyer
{
    class NetManager
    {
        public string GetPublicIp() //Funkcja pobierająca Publiczny adres IP
        {
            try
            {
                WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
                WebResponse response = request.GetResponse();
                StreamReader stream = new StreamReader(response.GetResponseStream());

                string adress = stream.ReadToEnd();

                //Szukanie IP w odebranym HTML
                int first = adress.IndexOf("Address: ") + 9; //Index litery HTML w której ma zacząć (czyli pierwsza cyfra ip, bo ono następuje po Address)
                int last = adress.LastIndexOf("</body>"); //Ostatnia litera wyrażenia body </body>
                adress = adress.Substring(first, last - first); //Wyciąga samego stringa od indexu w którym jest first, o długości ostatniego indexu <body> odjąć cały first
                return adress;
            }

            catch (Exception)
            {
                return "Błąd ustalenia publicznego IP";
            }
        }

        public string GetPrivateIp() //Funkcja pobierająca lokalny adres ip
        {
            IPHostEntry host;
            string adress = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    adress = ip.ToString();
                }
            }
            return adress;
        }
    }
}
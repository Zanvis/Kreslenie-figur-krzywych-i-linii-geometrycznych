using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjektNr3_Piwowarski62024
{
    public partial class ProjektIndywidualnyNr3 : Form
    {
        // deklaracje stałych
        const ushort PromienPunktu = 2;
        // deklaracje zmiennych referencyjnych narzędzi graficznych
        Graphics apRysownica;
        Graphics apRysownicaTymczasowa;
        Pen apPioro;
        Pen apPioroTymczasowe;
        SolidBrush apPedzel;
        Point apPunkt = Point.Empty;

        struct OpisKrzywejBeziera
        {
            public Point apPunktP0;
            public Point apPunktP1;
            public Point apPunktP2;
            public Point apPunktP3;
            public ushort apNumerPunktuKontrolnego;
            public ushort apPromienPunktuKontrolnego;
        }
        // deklaracja zmiennej dla KrzywejBeziera
        OpisKrzywejBeziera apKrzywaBeziera;

        struct OpisKrzywejKardynalnej
        {
            public ushort apLiczbaPunktow;
            public ushort apNumerPunktuKrzywej;
            public ushort apPromienPunktuKrzywej;
            public Point[] apPunktyKrzywej;
        }
        // deklaracja zmiennej dla KrzywejKardynalnej
        OpisKrzywejKardynalnej apKrzywaKardynalna;

        // deklaracja Fontu opisu punktu kontrolnego
        Font apFontOpisuPunktow = new Font("Arial", 8, FontStyle.Italic);

        public ProjektIndywidualnyNr3()
        {
            InitializeComponent();
            // ...
            // "podpięcie" BitMapy do kontrolki PictureBox
            pbRysownica.Image = new Bitmap(pbRysownica.Width, pbRysownica.Height);
            // utworzenie egzemplarza powierzchni graficznej
            apRysownica = Graphics.FromImage(pbRysownica.Image);
            // utworzenie egzemplarza tymczasowej powierzchni graficznej na przezroczystej powierzchni PictureBox
            apRysownicaTymczasowa = pbRysownica.CreateGraphics();
            // utworzenie egzemplarza Pióra tymczasowego
            apPioroTymczasowe = new Pen(Color.Black, 1.0f);
            // utworzenie egzemplarza Pióra i jego sformatowanie
            apPioro = new Pen(Color.Red, 1.7f);
            apPioro.DashStyle = DashStyle.Dash;
            apPioro.StartCap = LineCap.Round;
            apPioro.EndCap = LineCap.Round;

            // utworzenie egzemplarza Pedzla
            apPedzel = new SolidBrush(Color.RoyalBlue);
        }

        private void ProjektIndywidualnyNr3_FormClosing(object sender, FormClosingEventArgs e)
        {
            // utworzenie okna dialogowego dla potwierdzenia zamknięcia formularza indywidualnego
            DialogResult apOknoMessage = MessageBox.Show("Czy na pewno chcesz zamknąć ten formularz i przejść do formularza głównego?", this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);
            // rozpoznanie wybranej odpowiedzi Użytkownika programu
            if (apOknoMessage == DialogResult.Yes)
            {
                e.Cancel = false;
                // odszukanie egzemplarza formularza głównego w kolekcji OpenForms 
                foreach (Form apFormularz in Application.OpenForms)
                    // sprawdzamy, czy został znaleziony formularz główny
                    if (apFormularz.Name == "KokpitProjektuNr3")
                    {// ukrycie bieżącego formularza 
                        this.Hide();
                        // odsłonięcie znalezionego głównego formularza
                        apFormularz.Show();
                        // wyjście z metody obsługi zdarzenia FormClosing
                        return;
                    }
                // gdy będziemy tutaj, to będzie to oznaczało, że ktoś skasował formularz główny
                // utworzenie nowego egzemplarza formularza głównego KokpitProjektuNr3
                KokpitProjektuNr3 apFormularzKokpitProjektuNr3 = new KokpitProjektuNr3();
                // ukrycie bieżącego formularza 
                this.Hide();
                // odsłonięcie nowego formularza głównego
                this.Show();
            }
            else // anulujemy zmaknięcie formularza
                e.Cancel = true;
        }

        private void pbRysownica_MouseDown(object sender, MouseEventArgs e)
        {
            // wizualizacja współrzędnych aktualnego połóżenia myszy
            lblX.Text = e.Location.X.ToString();
            lblY.Text = e.Location.Y.ToString();
            // sprawdzenie czy zdarzenie MouseDown zostało spowodowane naciśnięciem lewego przycisku myszy
            if (e.Button == MouseButtons.Left)
                // tak, to zapamiętamy aktualne połóżenie Myszy
                apPunkt = e.Location;
        }

        private void pbRysownica_MouseUp(object sender, MouseEventArgs e)
        {
            // wizualizacja współrzędnych aktualnego połóżenia myszy
            lblX.Text = e.Location.X.ToString();
            lblY.Text = e.Location.Y.ToString();
            // ustalenie położenia i rozmiarów prostokąta, w którym będzie wkreślana figura
            int apLewyGornyNaroznikX = (apPunkt.X > e.Location.X) ? e.Location.X : apPunkt.X;
            int apLewyGornyNaroznikY = (apPunkt.Y > e.Location.Y) ? e.Location.Y : apPunkt.Y;
            int apSzerokosc = Math.Abs(apPunkt.X - e.Location.X);
            int apWysokosc = Math.Abs(apPunkt.Y - e.Location.Y);
            // sprawdzenie czy zdarzenie MouseUp zostało spowodowane zwolnieniem lewego przycisku myszy 
            if (e.Button == MouseButtons.Left)
            {// musimy teraz sprawdzić, która kontrolka RadioButton jest zaznaczona
                if (rdbProstokat.Checked)
                {
                    apRysownica.DrawRectangle(apPioro, apLewyGornyNaroznikX, apLewyGornyNaroznikY, apSzerokosc, apWysokosc);
                }
                if (rdbProstokatWypelniony.Checked)
                {
                    apPedzel.Color = btnKolorWypelnieniaFigur.BackColor;
                    apRysownica.FillRectangle(apPedzel, apLewyGornyNaroznikX, apLewyGornyNaroznikY, apSzerokosc, apWysokosc);
                }
                if (rdbKwadrat.Checked)
                {
                    apRysownica.DrawRectangle(apPioro, apLewyGornyNaroznikX, apLewyGornyNaroznikY, apSzerokosc, apSzerokosc);
                }
                if (rdbKwadratWypelniony.Checked)
                {
                    apPedzel.Color = btnKolorWypelnieniaFigur.BackColor;
                    apRysownica.FillRectangle(apPedzel, apLewyGornyNaroznikX, apLewyGornyNaroznikY, apSzerokosc, apSzerokosc);
                }
                if (rdbElipsa.Checked)
                {
                    apRysownica.DrawEllipse(apPioro, apLewyGornyNaroznikX, apLewyGornyNaroznikY, apSzerokosc, apWysokosc);
                }
                if (rdbElipsaWypelniona.Checked)
                {
                    apPedzel.Color = btnKolorWypelnieniaFigur.BackColor;
                    apRysownica.FillEllipse(apPedzel, apLewyGornyNaroznikX, apLewyGornyNaroznikY, apSzerokosc, apWysokosc);
                }
                if (rdbOkrag.Checked)
                {
                    apRysownica.DrawEllipse(apPioro, apLewyGornyNaroznikX, apLewyGornyNaroznikY, apSzerokosc, apSzerokosc);
                }
                if (rdbKolo.Checked)
                {
                    apPedzel.Color = btnKolorWypelnieniaFigur.BackColor;
                    apRysownica.FillEllipse(apPedzel, apLewyGornyNaroznikX, apLewyGornyNaroznikY, apSzerokosc, apSzerokosc);
                }
                if (rdbKrzywaBeziera.Checked)
                {
                    if (gbWyborKrzywej.Enabled)
                    {
                        // ustawienie stanu braku aktywności dla kontenera: gbWyborKrzywej
                        gbWyborKrzywej.Enabled = false;
                        apKrzywaBeziera.apNumerPunktuKontrolnego = 0;
                        // ustawienie stanu początkowego tworzonego opisu Krzywej Beziera
                        apKrzywaBeziera.apPromienPunktuKontrolnego = 5;
                        // przechowanie współrzędnych aktualnego położenia myszy
                        apKrzywaBeziera.apPunktP0 = e.Location;
                        // wizualizacja (wykreślenie) punktu P0
                        using (SolidBrush apPedzel = new SolidBrush(Color.Black))
                        {
                            // wykreślenie punktu P0
                            apRysownica.FillEllipse(apPedzel, e.Location.X - apKrzywaBeziera.apPromienPunktuKontrolnego, e.Location.Y - apKrzywaBeziera.apPromienPunktuKontrolnego, 2 * apKrzywaBeziera.apPromienPunktuKontrolnego, 2 * apKrzywaBeziera.apPromienPunktuKontrolnego);
                            // wykreślenie (wymalowanie) opisu Punktu P0
                            apRysownica.DrawString("p" + apKrzywaBeziera.apNumerPunktuKontrolnego.ToString(), apFontOpisuPunktow, apPedzel, e.Location);
                        }// tutaj nastąpi zwolnienie Pędzla
                    }// od if (gbWyborKrzywej.Enabled)
                    else
                    {// przechowanie współrzędnych kolejnych punktów kontrolnych
                     // zwiększenie numeru (licznika) punktów kontrolnych
                        apKrzywaBeziera.apNumerPunktuKontrolnego++;
                        // przechowanie wartości współrzędnych punktu kontrolnego o numerze w KrzywaBeziera.NumerPunktuKontrolnego
                        switch (apKrzywaBeziera.apNumerPunktuKontrolnego)
                        {
                            case 1: apKrzywaBeziera.apPunktP1 = e.Location; break;
                            case 2: apKrzywaBeziera.apPunktP2 = e.Location; break;
                            case 3: apKrzywaBeziera.apPunktP3 = e.Location; break;
                        }
                        // sprawdzenie, czy jest to już ostatni punkt Krzywej Beziera
                        if (apKrzywaBeziera.apNumerPunktuKontrolnego < 3)
                        {
                            // wykreślenie punktu kontrolnego krzywej Beziera
                            using (SolidBrush apPedzel = new SolidBrush(Color.Red))
                            {
                                apRysownica.FillEllipse(apPedzel, e.Location.X - apKrzywaBeziera.apPromienPunktuKontrolnego, e.Location.Y - apKrzywaBeziera.apPromienPunktuKontrolnego, 2 * apKrzywaBeziera.apPromienPunktuKontrolnego, 2 * apKrzywaBeziera.apPromienPunktuKontrolnego);
                                // wykreślenie (wymalowanie) opisu wykreślonego punktu kontrolnego
                                apRysownica.DrawString("p" + apKrzywaBeziera.apNumerPunktuKontrolnego.ToString(), apFontOpisuPunktow, apPedzel, e.Location);
                            }
                        }
                        else
                        {
                            // wykreślenie punktu końcowego krzywej Beziera
                            using (SolidBrush apPedzel = new SolidBrush(Color.Black))
                            {
                                apRysownica.FillEllipse(apPedzel, e.Location.X - apKrzywaBeziera.apPromienPunktuKontrolnego, e.Location.Y - apKrzywaBeziera.apPromienPunktuKontrolnego, 2 * apKrzywaBeziera.apPromienPunktuKontrolnego, 2 * apKrzywaBeziera.apPromienPunktuKontrolnego);
                                // wykreślenie (wymalowanie) opisu wykreślonego punktu kontrolnego
                                apRysownica.DrawString("p" + apKrzywaBeziera.apNumerPunktuKontrolnego.ToString(), apFontOpisuPunktow, apPedzel, e.Location);
                            }
                            // wykreślenie krzywej Beziera
                            apRysownica.DrawBezier(apPioro, apKrzywaBeziera.apPunktP0, apKrzywaBeziera.apPunktP1, apKrzywaBeziera.apPunktP2, apKrzywaBeziera.apPunktP3);
                            // ponowne uaktywnienie kontenera: gbWyborKrzywej
                            gbWyborKrzywej.Enabled = true;
                        }
                    }
                }
                if (rdbKrzywaKardynalna.Checked)
                {
                    if (gbWyborKrzywej.Enabled)
                    {
                        // ustawienie stanu braku aktywności dla kontenera: gbWyborKrzywej
                        gbWyborKrzywej.Enabled = false;
                        // zapisanie wartości liczby punktów podanej przez użytkownika
                        apKrzywaKardynalna.apLiczbaPunktow = (ushort)numUD_LiczbaPunktow.Value;
                        apKrzywaKardynalna.apNumerPunktuKrzywej = 0;

                        apKrzywaKardynalna.apPromienPunktuKrzywej = 5;
                        // stworzenie tablicy z tyloma wartościami, ile podał użytkownik
                        apKrzywaKardynalna.apPunktyKrzywej = new Point[apKrzywaKardynalna.apLiczbaPunktow];
                        // zapisanie lokalizacji pierwszego punktu
                        apKrzywaKardynalna.apPunktyKrzywej[0] = e.Location;
                        // narysowanie punktu
                        using (SolidBrush apPedzel = new SolidBrush(Color.Red))
                        {
                            apRysownica.FillEllipse(apPedzel, e.Location.X - apKrzywaKardynalna.apPromienPunktuKrzywej, e.Location.Y - apKrzywaKardynalna.apPromienPunktuKrzywej, 2 * apKrzywaKardynalna.apPromienPunktuKrzywej, 2 * apKrzywaKardynalna.apPromienPunktuKrzywej);
                            apRysownica.DrawString("p" + apKrzywaBeziera.apNumerPunktuKontrolnego.ToString(), apFontOpisuPunktow, apPedzel, e.Location);
                        }
                    }
                    else
                    {
                        apKrzywaKardynalna.apNumerPunktuKrzywej++;
                        // zapisanie lokalizacji każdych z punktów
                        switch (apKrzywaKardynalna.apNumerPunktuKrzywej)
                        {
                            case 1: apKrzywaKardynalna.apPunktyKrzywej[1] = e.Location; break;
                            case 2: apKrzywaKardynalna.apPunktyKrzywej[2] = e.Location; break;
                            case 3: apKrzywaKardynalna.apPunktyKrzywej[3] = e.Location; break;
                            case 4: apKrzywaKardynalna.apPunktyKrzywej[4] = e.Location; break;
                            case 5: apKrzywaKardynalna.apPunktyKrzywej[5] = e.Location; break;
                            case 6: apKrzywaKardynalna.apPunktyKrzywej[6] = e.Location; break;
                            case 7: apKrzywaKardynalna.apPunktyKrzywej[7] = e.Location; break;
                            case 8: apKrzywaKardynalna.apPunktyKrzywej[8] = e.Location; break;
                            case 9: apKrzywaKardynalna.apPunktyKrzywej[9] = e.Location; break;
                            case 10: apKrzywaKardynalna.apPunktyKrzywej[10] = e.Location; break;
                            case 11: apKrzywaKardynalna.apPunktyKrzywej[11] = e.Location; break;
                            case 12: apKrzywaKardynalna.apPunktyKrzywej[12] = e.Location; break;
                            case 13: apKrzywaKardynalna.apPunktyKrzywej[13] = e.Location; break;
                            case 14: apKrzywaKardynalna.apPunktyKrzywej[14] = e.Location; break;
                            case 15: apKrzywaKardynalna.apPunktyKrzywej[15] = e.Location; break;
                            case 16: apKrzywaKardynalna.apPunktyKrzywej[16] = e.Location; break;
                            case 17: apKrzywaKardynalna.apPunktyKrzywej[17] = e.Location; break;
                            case 18: apKrzywaKardynalna.apPunktyKrzywej[18] = e.Location; break;
                            case 19: apKrzywaKardynalna.apPunktyKrzywej[19] = e.Location; break;
                        }
                        if (apKrzywaKardynalna.apNumerPunktuKrzywej < apKrzywaKardynalna.apLiczbaPunktow - 1)
                        {
                            // narysowanie punktu
                            using (SolidBrush apPedzel = new SolidBrush(Color.Red))
                            {
                                apRysownica.FillEllipse(apPedzel, e.Location.X - apKrzywaKardynalna.apPromienPunktuKrzywej, e.Location.Y - apKrzywaKardynalna.apPromienPunktuKrzywej, 2 * apKrzywaKardynalna.apPromienPunktuKrzywej, 2 * apKrzywaKardynalna.apPromienPunktuKrzywej);
                                // wykreślenie (wymalowanie) opisu wykreślonego punktu kontrolnego
                                apRysownica.DrawString("p" + apKrzywaKardynalna.apNumerPunktuKrzywej.ToString(), apFontOpisuPunktow, apPedzel, e.Location);
                            }
                        }
                        else
                        {
                            // narysowanie punktu
                            using (SolidBrush apPedzel = new SolidBrush(Color.Red))
                            {
                                apRysownica.FillEllipse(apPedzel, e.Location.X - apKrzywaKardynalna.apPromienPunktuKrzywej, e.Location.Y - apKrzywaKardynalna.apPromienPunktuKrzywej, 2 * apKrzywaKardynalna.apPromienPunktuKrzywej, 2 * apKrzywaKardynalna.apPromienPunktuKrzywej);
                                apRysownica.DrawString("p" + apKrzywaKardynalna.apNumerPunktuKrzywej.ToString(), apFontOpisuPunktow, apPedzel, e.Location);
                            }
                            // wykreślenie krzywej kardynalnej
                            apRysownica.DrawCurve(apPioro, apKrzywaKardynalna.apPunktyKrzywej);
                            // ponowne uaktywnienie kontenera: gbWyborKrzywej
                            gbWyborKrzywej.Enabled = true;
                        }
                    }
                }
                if (rdbWypelnionaObramowanaZamknietaKrzywaKardynalna.Checked)
                {
                    if (gbWyborKrzywej.Enabled)
                    {
                        // ustawienie stanu braku aktywności dla kontenera: gbWyborKrzywej
                        gbWyborKrzywej.Enabled = false;
                        // zapisanie wartości liczby punktów podanej przez użytkownika
                        apKrzywaKardynalna.apLiczbaPunktow = (ushort)numUD_LiczbaPunktow.Value;
                        apKrzywaKardynalna.apNumerPunktuKrzywej = 0;
                        apKrzywaKardynalna.apPromienPunktuKrzywej = 5;
                        // stworzenie tablicy z tyloma wartościami, ile podał użytkownik
                        apKrzywaKardynalna.apPunktyKrzywej = new Point[apKrzywaKardynalna.apLiczbaPunktow];
                        // zapisanie lokalizacji pierwszego punktu
                        apKrzywaKardynalna.apPunktyKrzywej[0] = e.Location;
                        // narysowanie punktu
                        using (SolidBrush apPedzel = new SolidBrush(Color.Red))
                        {
                            apRysownica.FillEllipse(apPedzel, e.Location.X - apKrzywaKardynalna.apPromienPunktuKrzywej, e.Location.Y - apKrzywaKardynalna.apPromienPunktuKrzywej, 2 * apKrzywaKardynalna.apPromienPunktuKrzywej, 2 * apKrzywaKardynalna.apPromienPunktuKrzywej);
                            apRysownica.DrawString("p" + apKrzywaBeziera.apNumerPunktuKontrolnego.ToString(), apFontOpisuPunktow, apPedzel, e.Location);
                        }
                    }
                    else
                    {
                        apKrzywaKardynalna.apNumerPunktuKrzywej++;
                        // zapisanie lokalizacji każdych z punktów
                        switch (apKrzywaKardynalna.apNumerPunktuKrzywej)
                        {
                            case 1: apKrzywaKardynalna.apPunktyKrzywej[1] = e.Location; break;
                            case 2: apKrzywaKardynalna.apPunktyKrzywej[2] = e.Location; break;
                            case 3: apKrzywaKardynalna.apPunktyKrzywej[3] = e.Location; break;
                            case 4: apKrzywaKardynalna.apPunktyKrzywej[4] = e.Location; break;
                            case 5: apKrzywaKardynalna.apPunktyKrzywej[5] = e.Location; break;
                            case 6: apKrzywaKardynalna.apPunktyKrzywej[6] = e.Location; break;
                            case 7: apKrzywaKardynalna.apPunktyKrzywej[7] = e.Location; break;
                            case 8: apKrzywaKardynalna.apPunktyKrzywej[8] = e.Location; break;
                            case 9: apKrzywaKardynalna.apPunktyKrzywej[9] = e.Location; break;
                            case 10: apKrzywaKardynalna.apPunktyKrzywej[10] = e.Location; break;
                            case 11: apKrzywaKardynalna.apPunktyKrzywej[11] = e.Location; break;
                            case 12: apKrzywaKardynalna.apPunktyKrzywej[12] = e.Location; break;
                            case 13: apKrzywaKardynalna.apPunktyKrzywej[13] = e.Location; break;
                            case 14: apKrzywaKardynalna.apPunktyKrzywej[14] = e.Location; break;
                            case 15: apKrzywaKardynalna.apPunktyKrzywej[15] = e.Location; break;
                            case 16: apKrzywaKardynalna.apPunktyKrzywej[16] = e.Location; break;
                            case 17: apKrzywaKardynalna.apPunktyKrzywej[17] = e.Location; break;
                            case 18: apKrzywaKardynalna.apPunktyKrzywej[18] = e.Location; break;
                            case 19: apKrzywaKardynalna.apPunktyKrzywej[19] = e.Location; break;
                        }
                        if (apKrzywaKardynalna.apNumerPunktuKrzywej < apKrzywaKardynalna.apLiczbaPunktow - 1)
                        {
                            // narysowanie punktu
                            using (SolidBrush apPedzel = new SolidBrush(Color.Red))
                            {
                                apRysownica.FillEllipse(apPedzel, e.Location.X - apKrzywaKardynalna.apPromienPunktuKrzywej, e.Location.Y - apKrzywaKardynalna.apPromienPunktuKrzywej, 2 * apKrzywaKardynalna.apPromienPunktuKrzywej, 2 * apKrzywaKardynalna.apPromienPunktuKrzywej);
                                // wykreślenie (wymalowanie) opisu wykreślonego punktu kontrolnego
                                apRysownica.DrawString("p" + apKrzywaKardynalna.apNumerPunktuKrzywej.ToString(), apFontOpisuPunktow, apPedzel, e.Location);
                            }
                        }
                        else
                        {
                            // narysowanie punktu
                            using (SolidBrush apPedzel = new SolidBrush(Color.Red))
                            {
                                apRysownica.FillEllipse(apPedzel, e.Location.X - apKrzywaKardynalna.apPromienPunktuKrzywej, e.Location.Y - apKrzywaKardynalna.apPromienPunktuKrzywej, 2 * apKrzywaKardynalna.apPromienPunktuKrzywej, 2 * apKrzywaKardynalna.apPromienPunktuKrzywej);
                                apRysownica.DrawString("p" + apKrzywaKardynalna.apNumerPunktuKrzywej.ToString(), apFontOpisuPunktow, apPedzel, e.Location);
                            }

                            apPedzel.Color = btnKolorWypelnieniaKrzywej.BackColor;
                            // wykreślenie zamkniętej krzywej kardynalnej, z wypełnieniem albo obramowaniem
                            if (chbWypelnienieKrzywej.Checked && chbObramowanieKrzywej.Checked)
                            {
                                apRysownica.FillClosedCurve(apPedzel, apKrzywaKardynalna.apPunktyKrzywej);
                                apRysownica.DrawClosedCurve(apPioro, apKrzywaKardynalna.apPunktyKrzywej);
                            }
                            else if (chbWypelnienieKrzywej.Checked)
                                apRysownica.FillClosedCurve(apPedzel, apKrzywaKardynalna.apPunktyKrzywej);
                            else
                                apRysownica.DrawClosedCurve(apPioro, apKrzywaKardynalna.apPunktyKrzywej);
                            // ponowne uaktywnienie kontenera: gbWyborKrzywej
                            gbWyborKrzywej.Enabled = true;
                        }
                    }
                }
                if (rdbWycinekElipsy.Checked)
                {
                    // zapisanie kątów podanych przez użytkownika
                    int apPoczatkowyKat = (int)numUD_PoczatkowyKat.Value;
                    int apLiczbaStopni = (int)numUD_LiczbaStopni.Value;
                    // zabezpieczenie przed błędem
                    try
                    {
                        // wykreślenie wycinka elipsy
                        apRysownica.DrawPie(apPioro, apLewyGornyNaroznikX, apLewyGornyNaroznikY, apSzerokosc, apWysokosc, apPoczatkowyKat, apLiczbaStopni);
                    }
                    catch (ArgumentException) { }
                }
                if (rdbWypelnionyWycinekElipsy.Checked)
                {
                    // zapisanie kątów podanych przez użytkownika
                    int apPoczatkowyKat = (int)numUD_PoczatkowyKat.Value;
                    int apLiczbaStopni = (int)numUD_LiczbaStopni.Value;

                    apPedzel.Color = btnKolorWypelnieniaWycinka.BackColor;
                    // zabezpieczenie przed błędem
                    try
                    {
                        // wykreślenie wycinka elipsy z wypełnieniem albo obramowaniem
                        if (chbWypelnienieWycinka.Checked && chbObramowanieWycinka.Checked)
                        {
                            apRysownica.FillPie(apPedzel, apLewyGornyNaroznikX, apLewyGornyNaroznikY, apSzerokosc, apWysokosc, apPoczatkowyKat, apLiczbaStopni);
                            apRysownica.DrawPie(apPioro, apLewyGornyNaroznikX, apLewyGornyNaroznikY, apSzerokosc, apWysokosc, apPoczatkowyKat, apLiczbaStopni);
                        }
                        else if (chbWypelnienieWycinka.Checked)
                            apRysownica.FillPie(apPedzel, apLewyGornyNaroznikX, apLewyGornyNaroznikY, apSzerokosc, apWysokosc, apPoczatkowyKat, apLiczbaStopni);
                        else
                            apRysownica.DrawPie(apPioro, apLewyGornyNaroznikX, apLewyGornyNaroznikY, apSzerokosc, apWysokosc, apPoczatkowyKat, apLiczbaStopni);
                    }
                    catch (ArgumentException) { }
                }
                if (rdbLukElipsy.Checked)
                {
                    // zapisanie kątów podanych przez użytkownika
                    int apPoczatkowyKat = (int)numUD_PoczatkowyKat.Value;
                    int apLiczbaStopni = (int)numUD_LiczbaStopni.Value;
                    // zabezpieczenie przed błędem
                    try
                    { 
                        // wykreślenie łuku elipsy
                        apRysownica.DrawArc(apPioro, apLewyGornyNaroznikX, apLewyGornyNaroznikY, apSzerokosc, apWysokosc, apPoczatkowyKat, apLiczbaStopni);
                    }
                    catch (ArgumentException) { }
                }
            }
            // odświerzenie powierzchni graficznej
            pbRysownica.Refresh();
        }

        private void pbRysownica_MouseMove(object sender, MouseEventArgs e)
        {
            //wizualizacja współrzędnych aktualnego połóżenia myszy
            lblX.Text = e.Location.X.ToString();
            lblY.Text = e.Location.Y.ToString();
            // ustalenie położenia i rozmiarów prostokąta, w którym będzie wkreślana figura
            int apLewyGornyNaroznikX = (apPunkt.X > e.Location.X) ? e.Location.X : apPunkt.X;
            int apLewyGornyNaroznikY = (apPunkt.Y > e.Location.Y) ? e.Location.Y : apPunkt.Y;
            int apSzerokosc = Math.Abs(apPunkt.X - e.Location.X);
            int apWysokosc = Math.Abs(apPunkt.Y - e.Location.Y);

            if (e.Button == MouseButtons.Left)
            {
                // wykreślenie "poświat" pomagających rysować figury
                if (rdbProstokat.Checked || rdbProstokatWypelniony.Checked)
                {
                    apRysownicaTymczasowa.DrawRectangle(apPioroTymczasowe, apLewyGornyNaroznikX, apLewyGornyNaroznikY, apSzerokosc, apWysokosc);
                }
                if (rdbKwadrat.Checked || rdbKwadratWypelniony.Checked)
                {
                    apRysownicaTymczasowa.DrawRectangle(apPioroTymczasowe, apLewyGornyNaroznikX, apLewyGornyNaroznikY, apSzerokosc, apSzerokosc);
                }
                if (rdbElipsa.Checked || rdbElipsaWypelniona.Checked)
                {
                    apRysownicaTymczasowa.DrawEllipse(apPioroTymczasowe, apLewyGornyNaroznikX, apLewyGornyNaroznikY, apSzerokosc, apWysokosc);
                }
                if (rdbOkrag.Checked || rdbKolo.Checked)
                {
                    apRysownicaTymczasowa.DrawEllipse(apPioroTymczasowe, apLewyGornyNaroznikX, apLewyGornyNaroznikY, apSzerokosc, apSzerokosc);
                }
                if (rdbWycinekElipsy.Checked || rdbWypelnionyWycinekElipsy.Checked)
                {
                    int apPoczatkowyKat = (int)numUD_PoczatkowyKat.Value;
                    int apLiczbaStopni = (int)numUD_LiczbaStopni.Value;
                    try
                    {
                        apRysownicaTymczasowa.DrawPie(apPioroTymczasowe, apLewyGornyNaroznikX, apLewyGornyNaroznikY, apSzerokosc, apWysokosc, apPoczatkowyKat, apLiczbaStopni);
                    }
                    catch (ArgumentException) { }
                }
                if (rdbLukElipsy.Checked)
                {
                    int apPoczatkowyKat = (int)numUD_PoczatkowyKat.Value;
                    int apLiczbaStopni = (int)numUD_LiczbaStopni.Value;

                    try
                    {
                        apRysownicaTymczasowa.DrawArc(apPioroTymczasowe, apLewyGornyNaroznikX, apLewyGornyNaroznikY, apSzerokosc, apWysokosc, apPoczatkowyKat, apLiczbaStopni);
                    }
                    catch (ArgumentException) { }
                }
            }
            // odświerzenie powierzchni graficznej
            pbRysownica.Refresh();
        }

        private void btnKolorWypelnieniaFigur_Click(object sender, EventArgs e)
        {
            ColorDialog apPaletaKolorow = new ColorDialog();
            apPaletaKolorow.Color = btnKolorWypelnieniaFigur.BackColor;
            // wyświetlenie Palety kolorów
            if (apPaletaKolorow.ShowDialog() == DialogResult.OK)
                btnKolorWypelnieniaFigur.BackColor = apPaletaKolorow.Color;
            // zwolnienie zasobu graficznego, czyli Palety kolorów
            apPaletaKolorow.Dispose();
        }

        private void btnKolorWypelnieniaKrzywej_Click(object sender, EventArgs e)
        {
            ColorDialog apPaletaKolorow = new ColorDialog();
            apPaletaKolorow.Color = btnKolorWypelnieniaKrzywej.BackColor;
            // wyświetlenie Palety kolorów
            if (apPaletaKolorow.ShowDialog() == DialogResult.OK)
                btnKolorWypelnieniaKrzywej.BackColor = apPaletaKolorow.Color;
            // zwolnienie zasobu graficznego, czyli Palety kolorów
            apPaletaKolorow.Dispose();
        }

        private void btnKolorWypelnieniaWycinka_Click(object sender, EventArgs e)
        {
            ColorDialog apPaletaKolorow = new ColorDialog();
            apPaletaKolorow.Color = btnKolorWypelnieniaWycinka.BackColor;
            // wyświetlenie Palety kolorów
            if (apPaletaKolorow.ShowDialog() == DialogResult.OK)
                btnKolorWypelnieniaWycinka.BackColor = apPaletaKolorow.Color;
            // zwolnienie zasobu graficznego, czyli Palety kolorów
            apPaletaKolorow.Dispose();
        }

        private void rdbProstokatWypelniony_CheckedChanged(object sender, EventArgs e)
        {
            // ustawienie widoczności przycisku
            if (rdbProstokatWypelniony.Checked)
            {
                btnKolorWypelnieniaFigur.Visible = true;
            }
            else
            {
                btnKolorWypelnieniaFigur.Visible = false;
            }
        }

        private void rdbKwadratWypelniony_CheckedChanged(object sender, EventArgs e)
        {
            // ustawienie widoczności przycisku
            if (rdbKwadratWypelniony.Checked)
            {
                btnKolorWypelnieniaFigur.Visible = true;
            }
            else
            {
                btnKolorWypelnieniaFigur.Visible = false;
            }
        }

        private void rdbElipsaWypelniona_CheckedChanged(object sender, EventArgs e)
        {
            // ustawienie widoczności przycisku
            if (rdbElipsaWypelniona.Checked)
            {
                btnKolorWypelnieniaFigur.Visible = true;
            }
        }

        private void rdbKolo_CheckedChanged(object sender, EventArgs e)
        {
            // ustawienie widoczności przycisku
            if (rdbKolo.Checked)
            {
                btnKolorWypelnieniaFigur.Visible = true;
            }
            else
            {
                btnKolorWypelnieniaFigur.Visible = false;
            }
        }

        private void rdbKrzywaBeziera_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbKrzywaBeziera.Checked)
                // wizualizacja okna dialogowego z informacją dla Użytkownika: co powinien zrobić?
                MessageBox.Show("Wykreślenie krzywej Beziera wymaga zaznaczenia (kliknięcia) 4 punktów na Rysownicy", "Kreślenie krzywej Beziera", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void rdbKrzywaKardynalna_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbKrzywaKardynalna.Checked)
            {
                // wyświetlenie okna dialogowego dla użytkownika programu
                MessageBox.Show("Wykreślenie krzywej kardynalnej wymaga ustalenia (podania) liczby punktów (minimalna liczba punktów krzywej kardynalnej, to 3!)",
                    "Kreślenie krzywej kardynalnej", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // ustawienie widoczności oraz limitów
                numUD_LiczbaPunktow.Visible = true;
                lblLiczbaPunktow.Visible = true;
                numUD_LiczbaPunktow.Minimum = 3;
                numUD_LiczbaPunktow.Maximum = 20;
            }
            else
            {
                // ustawienie widoczności
                numUD_LiczbaPunktow.Visible = false;
                lblLiczbaPunktow.Visible = false;
            }
        }
        private void rdbWypelnionaObramowanaZamknietaKrzywaKardynalna_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbWypelnionaObramowanaZamknietaKrzywaKardynalna.Checked)
            {
                // wyświetlenie okna dialogowego dla użytkownika programu
                MessageBox.Show("Wykreślenie zamkniętej krzywej kardynalnej wymaga ustalenia (podania) liczby punktów (minimalna liczba punktów krzywej kardynalnej, to 3!)",
                    "Kreślenie zamkniętej krzywej kardynalnej", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // ustawienie widoczności oraz limitów
                numUD_LiczbaPunktow.Visible = true;
                chbObramowanieKrzywej.Visible = true;
                chbWypelnienieKrzywej.Visible = true;
                btnKolorWypelnieniaKrzywej.Visible = true;
                lblLiczbaPunktow.Visible = true;
                numUD_LiczbaPunktow.Minimum = 3;
                numUD_LiczbaPunktow.Maximum = 20;
            }
            else
            {
                // ustawienie widoczności
                numUD_LiczbaPunktow.Visible = false;
                chbObramowanieKrzywej.Visible = false;
                chbWypelnienieKrzywej.Visible = false;
                btnKolorWypelnieniaKrzywej.Visible = false;
                lblLiczbaPunktow.Visible = false;
            }
        }

        private void rdbWypelnionyWycinekElipsy_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbWypelnionyWycinekElipsy.Checked)
            {
                // wyświetlenie okna dialogowego dla użytkownika programu
                MessageBox.Show("Wykreślenie wycinka elipsy wymaga ustalenia (podania) kąta początkowego i kąta nachylenia (minimalnie 1 stopień, maksymalnie 360 stopni)",
                    "Kreślenie wycinka elispy", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // ustawienie widoczności oraz limitów
                numUD_LiczbaStopni.Visible = true;
                numUD_PoczatkowyKat.Visible = true;
                chbObramowanieWycinka.Visible = true;
                chbWypelnienieWycinka.Visible = true;
                btnKolorWypelnieniaWycinka.Visible = true;
                lblKatPoczatkowy.Visible = true;
                lblKatNachylenia.Visible = true;
                numUD_LiczbaStopni.Minimum = 1;
                numUD_PoczatkowyKat.Minimum = 1;
                numUD_LiczbaStopni.Maximum = 360;
                numUD_PoczatkowyKat.Maximum = 360;
            }
            else
            {
                // ustawienie widoczności
                numUD_LiczbaStopni.Visible = false;
                numUD_PoczatkowyKat.Visible = false;
                chbObramowanieWycinka.Visible = false;
                chbWypelnienieWycinka.Visible = false;
                btnKolorWypelnieniaWycinka.Visible = false;
                lblKatPoczatkowy.Visible = false;
                lblKatNachylenia.Visible = false;
            }
        }

        private void rdbWycinekElipsy_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbWycinekElipsy.Checked)
            {
                // wyświetlenie okna dialogowego dla użytkownika programu
                MessageBox.Show("Wykreślenie wycinka elipsy wymaga ustalenia (podania) kąta początkowego i kąta nachylenia (minimalnie 1 stopień, maksymalnie 360 stopni)",
                    "Kreślenie wycinka elispy", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // ustawienie widoczności oraz limitów
                numUD_LiczbaStopni.Visible = true;
                numUD_PoczatkowyKat.Visible = true;
                lblKatPoczatkowy.Visible = true;
                lblKatNachylenia.Visible = true;
                numUD_LiczbaStopni.Minimum = 1;
                numUD_PoczatkowyKat.Minimum = 1;
                numUD_LiczbaStopni.Maximum = 360;
                numUD_PoczatkowyKat.Maximum = 360;
            }
            else
            {
                // ustawienie widoczności
                numUD_LiczbaStopni.Visible = false;
                numUD_PoczatkowyKat.Visible = false;
                lblKatPoczatkowy.Visible = false;
                lblKatNachylenia.Visible = false;
            }
        }

        private void rdbLukElipsy_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbLukElipsy.Checked)
            {
                // wyświetlenie okna dialogowego dla użytkownika programu
                MessageBox.Show("Wykreślenie łuku elipsy wymaga ustalenia (podania) kąta początkowego i kąta nachylenia (minimalnie 1 stopień, maksymalnie 360 stopni)",
                    "Kreślenie łuku elispy", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // ustawienie widoczności oraz limitów
                numUD_LiczbaStopni.Visible = true;
                numUD_PoczatkowyKat.Visible = true;
                lblKatPoczatkowy.Visible = true;
                lblKatNachylenia.Visible = true;
                numUD_LiczbaStopni.Minimum = 1;
                numUD_PoczatkowyKat.Minimum = 1;
                numUD_LiczbaStopni.Maximum = 360;
                numUD_PoczatkowyKat.Maximum = 360;
            }
            else
            {
                // ustawienie widoczności
                numUD_LiczbaStopni.Visible = false;
                numUD_PoczatkowyKat.Visible = false;
                lblKatPoczatkowy.Visible = false;
                lblKatNachylenia.Visible = false;
            }
        }
        private void zapiszBitMapęWPlikuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // zapisanie rysownicy w pliku
            using (SaveFileDialog apPlikDoZapisu = new SaveFileDialog() { Filter = @"PNG|*.png" })
            {
                if (apPlikDoZapisu.ShowDialog() == DialogResult.OK)
                {
                    pbRysownica.Image.Save(apPlikDoZapisu.FileName);
                }
            }
        }

        private void odczytajBitMapęZPlikuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // odczytanie rysownicy z pliku
            OpenFileDialog apPlikDoOdczytu = new OpenFileDialog();
            apPlikDoOdczytu.ShowDialog();
            string sciezka = apPlikDoOdczytu.FileName;
            try
            {
                pbRysownica.Image = Image.FromFile(sciezka);
            }
            catch (ArgumentException) { }

            apRysownica = Graphics.FromImage(pbRysownica.Image);
        }

        private void exitZFormularzaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // zamknięcie formularza
            Close();
        }

        private void restartProgramuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // restart programu
            Application.Restart();
        }

        private void kolorLiniiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog apPaletaKolorow = new ColorDialog();
            apPaletaKolorow.Color = apPioro.Color;
            // wyświetlenie Palety kolorów
            if (apPaletaKolorow.ShowDialog() == DialogResult.OK)
                apPioro.Color = apPaletaKolorow.Color;
            // zwolnienie zasobu graficznego, czyli Palety kolorów
            apPaletaKolorow.Dispose();
        }

        private void kolorTłaRysownicyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog apPaletaKolorow = new ColorDialog();
            apPaletaKolorow.Color = pbRysownica.BackColor;
            // wyświetlenie Palety kolorów
            if (apPaletaKolorow.ShowDialog() == DialogResult.OK)
                pbRysownica.BackColor = apPaletaKolorow.Color;
            // zwolnienie zasobu graficznego, czyli Palety kolorów
            apPaletaKolorow.Dispose();
        }

        private void kolorWypełnieniaFormularzaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog apPaletaKolorow = new ColorDialog();
            apPaletaKolorow.Color = this.BackColor;
            // wyświetlenie Palety kolorów
            if (apPaletaKolorow.ShowDialog() == DialogResult.OK)
                this.BackColor = apPaletaKolorow.Color;
            // zwolnienie zasobu graficznego, czyli Palety kolorów
            apPaletaKolorow.Dispose();
        }
        // ustawienie grubości pióra
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            apPioro.Width = 1;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            apPioro.Width = 2;
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            apPioro.Width = 3;
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            apPioro.Width = 4;
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            apPioro.Width = 5;
        }
        // ustawienie grotu linii
        // ustawienie grotu linii
        private void liniaKreskowaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            apPioro.DashStyle = DashStyle.Dash;
        }

        private void liniaKreskowoKropkowaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            apPioro.DashStyle = DashStyle.DashDot;
        }

        private void liniaKreskowoKropkowaKropkowaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            apPioro.DashStyle = DashStyle.DashDotDot;
        }

        private void liniaKropkowaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            apPioro.DashStyle = DashStyle.Dot;
        }

        private void ciągłaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            apPioro.DashStyle = DashStyle.Solid;
        }

        private void strzałkaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            apPioro.StartCap = LineCap.ArrowAnchor;
        }

        private void diamentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            apPioro.StartCap = LineCap.DiamondAnchor;
        }

        private void zaokToolStripMenuItem_Click(object sender, EventArgs e)
        {
            apPioro.StartCap = LineCap.RoundAnchor;
        }

        private void kwadratToolStripMenuItem_Click(object sender, EventArgs e)
        {
            apPioro.StartCap = LineCap.SquareAnchor;
        }

        private void brakToolStripMenuItem_Click(object sender, EventArgs e)
        {
            apPioro.StartCap = LineCap.Round;
        }

        private void strzałkaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            apPioro.EndCap = LineCap.ArrowAnchor;
        }

        private void diamentToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            apPioro.EndCap = LineCap.DiamondAnchor;
        }

        private void zaokrąglenieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            apPioro.EndCap = LineCap.RoundAnchor;
        }

        private void kwadratToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            apPioro.EndCap = LineCap.SquareAnchor;
        }

        private void brakToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            apPioro.EndCap = LineCap.Round;
        }

        private void kontrolkaGroupBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // utworzenie okna dialogowego dla zmiany czcionki
            FontDialog apOknoCzcionki = new FontDialog();
            // zaznaczenie w oknie OknoCzcionki bieżącego fontu
            apOknoCzcionki.Font = gbWyborKrzywej.Font;
            // wyświetlenie okna dialogowego OknoCzcionki i "odczytanie" nowych ustawień dla fontów
            if (apOknoCzcionki.ShowDialog() == DialogResult.OK)
            {
                // ustawienie nowego fontu da formularza
                gbWyborKrzywej.Font = apOknoCzcionki.Font;
                rdbElipsa.Font = apOknoCzcionki.Font;
                rdbElipsaWypelniona.Font = apOknoCzcionki.Font; 
                rdbKolo.Font = apOknoCzcionki.Font;
                rdbKrzywaBeziera.Font = apOknoCzcionki.Font;
                rdbKrzywaKardynalna.Font = apOknoCzcionki.Font;
                rdbKwadrat.Font = apOknoCzcionki.Font;
                rdbKwadratWypelniony.Font = apOknoCzcionki.Font;
                rdbLukElipsy.Font = apOknoCzcionki.Font;
                rdbOkrag.Font = apOknoCzcionki.Font;
                rdbProstokat.Font = apOknoCzcionki.Font;
                rdbProstokatWypelniony.Font = apOknoCzcionki.Font;
                rdbWycinekElipsy.Font = apOknoCzcionki.Font;
                rdbWypelnionaObramowanaZamknietaKrzywaKardynalna.Font = apOknoCzcionki.Font;
                rdbWypelnionyWycinekElipsy.Font = apOknoCzcionki.Font;
            }
            // zwolnienie przydzielonego zasobu pamięci dla apOknoCzcionki
            apOknoCzcionki.Dispose();
        }

        private void kontrolkaLabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // utworzenie okna dialogowego dla zmiany czcionki
            FontDialog apOknoCzcionki = new FontDialog();
            // zaznaczenie w oknie OknoCzcionki bieżącego fontu
            apOknoCzcionki.Font = lblLiczbaPunktow.Font;
            // wyświetlenie okna dialogowego OknoCzcionki i "odczytanie" nowych ustawień dla fontów
            if (apOknoCzcionki.ShowDialog() == DialogResult.OK)
            {
                // ustawienie nowego fontu da formularza
                lblKatNachylenia.Font = apOknoCzcionki.Font;
                lblKatPoczatkowy.Font = apOknoCzcionki.Font;
                lblLiczbaPunktow.Font = apOknoCzcionki.Font;
                lblX.Font = apOknoCzcionki.Font;
                lblY.Font = apOknoCzcionki.Font;
                lblWspolrzedne.Font = apOknoCzcionki.Font;
            }
            // zwolnienie przydzielonego zasobu pamięci dla apOknoCzcionki
            apOknoCzcionki.Dispose();
        }

        private void kontrolkaGroupBoxToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // otworzenie okna dialogowego z paletą kolorów
            ColorDialog apPaletaKolorow = new ColorDialog();
            // zaznaczenie w PalecieKolorow bieżącego koloru tła wykresu
            apPaletaKolorow.Color = gbWyborKrzywej.ForeColor;
            // wyświetlenie palety kolorów i "odczytanie" wybranego koloru przez Użytkownika
            if (apPaletaKolorow.ShowDialog() == DialogResult.OK)
                // dokonujemy zmiany koloru czcionki
                gbWyborKrzywej.ForeColor = apPaletaKolorow.Color;
            // zwolnienie egzemplarza apPaletyKolorow
            apPaletaKolorow.Dispose();
        }

        private void kontrolkaLabelToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // otworzenie okna dialogowego z paletą kolorów
            ColorDialog apPaletaKolorow = new ColorDialog();
            // zaznaczenie w PalecieKolorow bieżącego koloru tła wykresu
            apPaletaKolorow.Color = lblLiczbaPunktow.ForeColor;
            // wyświetlenie palety kolorów i "odczytanie" wybranego koloru przez Użytkownika
            if (apPaletaKolorow.ShowDialog() == DialogResult.OK)
            {
                // dokonujemy zmiany koloru czcionki
                lblLiczbaPunktow.ForeColor = apPaletaKolorow.Color;
                lblKatNachylenia.ForeColor = apPaletaKolorow.Color;
                lblKatPoczatkowy.ForeColor = apPaletaKolorow.Color;
                lblX.ForeColor = apPaletaKolorow.Color;
                lblY.ForeColor = apPaletaKolorow.Color;
                lblWspolrzedne.ForeColor = apPaletaKolorow.Color;
            }
            // zwolnienie egzemplarza apPaletyKolorow
            apPaletaKolorow.Dispose();
        }
    }
}

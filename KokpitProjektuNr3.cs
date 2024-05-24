using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjektNr3_Piwowarski62024
{
    public partial class KokpitProjektuNr3 : Form
    {
        public KokpitProjektuNr3()
        {
            InitializeComponent();
        }

        private void btnLaboratoriumNr3_Click(object sender, EventArgs e)
        {
            foreach (Form Formularz in Application.OpenForms)
                if (Formularz.Name == "LaboratoriumNr3")
                {
                    this.Hide();
                    Formularz.Show();
                    return;
                }
            // utworzenie egzemplarza formularza: LaboratoriumNr3
            LaboratoriumNr3 FormularzLaboratoryjny = new LaboratoriumNr3();
            this.Hide();
            FormularzLaboratoryjny.Show();
        }

        private void btnProjektIndywidualnyNr3_Click(object sender, EventArgs e)
        {
            foreach (Form Formularz in Application.OpenForms)
                if (Formularz.Name == "ProjektIndywidualnyNr3")
                {
                    this.Hide();
                    Formularz.Show();
                    return;
                }
            // utworzenie egzemplarza formularza: ProjektIndywidualnyNr3
            ProjektIndywidualnyNr3 FormularzIndywidualny = new ProjektIndywidualnyNr3();
            this.Hide();
            FormularzIndywidualny.Show();
        }
    }
}

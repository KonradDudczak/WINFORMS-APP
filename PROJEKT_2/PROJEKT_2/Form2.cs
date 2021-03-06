using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROJEKT_2
{
    public partial class Form2 : Form
    {
        private Form1 form1;

        private string private_key;

        public static string public_key;

        public static string algorithm = "SHA256";

        public static byte[] signature;
        public Form2()
        {
            InitializeComponent();
            form1 = new Form1();
        }

        public void key_generator()
        {
            CspParameters csp = new CspParameters();
            csp.KeyContainerName = "MyKeys";
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(csp);
            private_key = rsa.ToXmlString(true);
            public_key = rsa.ToXmlString(false);

        }

        private void buttonClicked(object sender, EventArgs e)
        {
            
            this.form1.Location = this.Location;
            this.form1.StartPosition = FormStartPosition.Manual;
            StreamWriter sw = new StreamWriter(Form1.textFile);
            sw.Write(textBox1.Text);
            sw.Dispose();
            string text = File.ReadAllText(Form1.textFile);
            key_generator();
            RSACryptoServiceProvider rsa_with_private = new RSACryptoServiceProvider();
            rsa_with_private.FromXmlString(private_key);
            byte[] data = (new System.Text.ASCIIEncoding()).GetBytes(text);
            signature = rsa_with_private.SignData(data, algorithm);
            this.Hide();

            string message = Form2.public_key.ToString();
            string caption = "KLUCZ PUBLICZNY ODBIORCY";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBox.Show(message, caption, buttons);

            message = private_key.ToString();
            caption = "KLUCZ PRYWATNY ODBIORCY";
            buttons = MessageBoxButtons.OK;
            MessageBox.Show(message, caption, buttons);

            message = "Otrzymałeś nową wiadomość";
            caption = "NOWA WIADOMOŚĆ";
            buttons = MessageBoxButtons.OK;
            MessageBox.Show(message, caption, buttons);


            this.form1.ShowDialog();
            
        }

        private void buttonwyczyscClicked(object sender, EventArgs e)
        {
            textBox1.Clear();
            FileStream fileStream = File.Open(Form1.textFile, FileMode.Open);
            fileStream.SetLength(0);
            fileStream.Close(); 
        }

        private void onSprawdzClicked(object sender, EventArgs e)
        {
            string text = File.ReadAllText(Form1.textFile);

            if (text.Length == 0)
            {
                string message = "Pusta widomość";
                string caption = "KOMUNIKAT";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons);
            }


            else
            {


                textBox1.Text = text;

                byte[] data = (new System.Text.ASCIIEncoding()).GetBytes(text);


                RSACryptoServiceProvider rsa_only_public = new RSACryptoServiceProvider();
                rsa_only_public.FromXmlString(Form1.public_key);

                if (rsa_only_public.VerifyData(data, Form1.algorithm, Form1.signature))
                {
                    string message = "Ta wiadomość została podpisana przez zaufanego nadawcę";
                    string caption = "Weryfikacja podpisu";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(message, caption, buttons);

                }

                else
                {
                    string message = "Ta wiadomość NIE została podpisana przez zaufanego nadawcę";
                    string caption = "Weryfikacja podpisu";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(message, caption, buttons);
                }
            }
            
        }
    }
}

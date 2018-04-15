using System;
using System.Text;
using System.Windows.Forms;
using Kzar.ASN1.BER;
using System.Threading;

namespace Client
{
    public partial class MainWindow : Form
    {
        private static Crypter crypter = new Crypter();
        private static ClientSocket handler = new ClientSocket();
        private enum Cmd { certs, cipher, sign, error };
        private Thread clientThreadSender;
        static byte[] data;




        public MainWindow()
        {
            try
            {
                InitializeComponent();

                ClientSocket.lockObject = new Object();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        private void SignOperationTo()
        {
            try
            {

                var operation = DetermineOpearation();
                var operationAsn1 = Asn1Formatter.SetCommandAsn1((int)Cmd.sign);
                handler.Send(operationAsn1);
                data = handler.Recieve();
                if (Encoding.ASCII.GetString(Asn1Formatter.GetCertAsn1(data)) == "ESTABLISHED")
                {
                    int certIndex = (int)CerificatesBox.Invoke(new Func<int>(() => CerificatesBox.SelectedIndex));
                    handler.Send(crypter.Sign(Encoding.ASCII.GetBytes(textBox1.Text), certIndex));
                }
                else
                {
                    throw new Exception("Error while send");
                }

            }
            catch (NullReferenceException ne)
            {
                var errorCode = Asn1Formatter.SetCommandAsn1((int)Cmd.error);
                handler.Send(errorCode);
                MessageBox.Show(ne.Message);
                Console.WriteLine(ne.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }



        }
        private void EncryptOperationTo()
        {
            try
            {
                crypter = new Crypter();
                var operation = DetermineOpearation();
                var operationAsn1 = Asn1Formatter.SetCommandAsn1((int)Cmd.cipher);
                byte[] buffer;
                handler.Send(operationAsn1);
                data = handler.Recieve();
                var text = (string)CerificatesBox.Invoke(new Func<string>(() => CerificatesBox.Text));
                var asn1Cert = Asn1Formatter.SetCertNameAsn1(Encoding.ASCII.GetBytes(text));
                handler.Send(asn1Cert);
                data = handler.Recieve();
                byte[] cert = Asn1Formatter.GetCertAsn1(data);
                crypter.FromBytesToCert(cert);
                byte[] symivBytes = Asn1Formatter.SetSymKeyAndIVAsn1(crypter.GetEncryptedSymKey(), crypter.IV);
                handler.Send(symivBytes);
                data = handler.Recieve();
                if (Encoding.ASCII.GetString(Asn1Formatter.GetCertAsn1(data)) == "ESTABLISHED")
                {
                    text = (string)textBox1.Invoke(new Func<string>(() => textBox1.Text));
                    buffer = crypter.Encrypt(Encoding.ASCII.GetBytes(text));
                    handler.Send(buffer);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }



        private String DetermineOpearation()
        {

            if (Cipher.Checked)
            {
                return "Cipher";
            }
            else
            {

                if (Sign.Checked)
                {
                    return "Sign";
                }
            }

            return null;

        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Cipher_CheckedChanged(object sender, EventArgs e)
        {
            if (Cipher.Checked)
            {
                var asn1Cmd = Asn1Formatter.SetCommandAsn1((int)Cmd.certs);
                handler.Send(asn1Cmd);
                data = handler.Recieve();
                BERelement certsNames = BERelement.DecodePacket(data);
                CerificatesBox.DataSource = null;
                CerificatesBox.Items.Clear();
                foreach (var cert in certsNames.Items)
                {
                    CerificatesBox.Items.Add(Encoding.ASCII.GetString(cert.Value));
                }
                CerificatesBox.SelectedIndex = 0;
            }
        }

        private void SendData(object sender, EventArgs e)
        {

            try
            {
                if (Cipher.Checked)
                {
                    if (textBox1.Text != "")
                    {
                        clientThreadSender = new Thread(() => EncryptOperationTo());
                        clientThreadSender.Start();

                    }
                    else
                    {
                        MessageBox.Show("Nothing to send!");
                    }
                }
                else
                {
                    if (Sign.Checked)
                    {
                        if (textBox1.Text != "")
                        {
                            clientThreadSender = new Thread(() => SignOperationTo());
                            clientThreadSender.Start();

                        }
                        else
                        {
                            MessageBox.Show("Nothing to send!");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void SignChecked(object sender, EventArgs e)
        {
            if (Sign.Checked)
            {
                CerificatesBox.DataSource = crypter.ClientCertificates;
                CerificatesBox.DisplayMember = "FriendlyName";
            }

        }

        private void ComputeHash(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                crypter.ComputeHash(Encoding.ASCII.GetBytes(textBox1.Text));
                HashBox.Text = BitConverter.ToString(crypter.GetHashValue()).Replace("-", "");
            }
            else
            {
                MessageBox.Show("Nothing to hash!");
            }

        }

        private void GetCertsFromServer(object sender, EventArgs e)
        {
            var asn1Cmd = Asn1Formatter.SetCommandAsn1((int)Cmd.certs);
            int.TryParse(textBox3.Text, out int port);
            handler = new ClientSocket(textBox2.Text, port);
            handler.Init();
            handler.Send(asn1Cmd);
            data = handler.Recieve();
            BERelement certsNames = BERelement.DecodePacket(data);
            CerificatesBox.DataSource = null;
            CerificatesBox.Items.Clear();
            foreach (var cert in certsNames.Items)
            {
                CerificatesBox.Items.Add(Encoding.ASCII.GetString(cert.Value));
            }
            CerificatesBox.SelectedIndex = 0;
        }

        private void IpClick(object sender, EventArgs e)
        {
            textBox2.Text = "";
        }

        private void PortClick(object sender, EventArgs e)
        {
            textBox3.Text = "";
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using Kzar.ASN1.BER;

namespace Client
{
    public partial class MainWindow : Form
    {


        private static Crypter crypter = new Crypter();
        private static ClientSocket handler = new ClientSocket();
        private enum Cmd { certs, cipher, hash, sign }

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                handler.Init();
                BERelement asn = new BERelement(0x30);
                asn.AddItem(new BERelement(0x02, (int)Cmd.certs));
                handler.Send(asn.GetEncodedPacket());
                byte[] data = handler.Recieve();
                BERelement certsNames = BERelement.DecodePacket(data);
                foreach (var cert in certsNames.Items)
                {
                    CerificatesBox.Items.Add(Encoding.ASCII.GetString(cert.Value));
                }
                CerificatesBox.SelectedIndex = 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                if (Hash.Checked)
                {
                    return "Hash";
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
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Cipher_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void SendData(object sender, EventArgs e)
        {
            try
            {

                string operation = DetermineOpearation();
                BERelement asn = new BERelement(0x30);
                asn.AddItem(new BERelement(0x04, Encoding.ASCII.GetBytes(CerificatesBox.SelectedText)));
                handler.Send(asn.GetEncodedPacket());


                //StringBuilder builder = new StringBuilder();
                //int bytes = 0; // количество полученных байт
                //do
                //{
                //    bytes = socket.Receive(data, data.Length, 0);
                //    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                //}
                //while (socket.Available > 0);

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}

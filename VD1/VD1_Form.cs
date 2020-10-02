using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

//KALMAN ROLAND
//2020.10

namespace VD1
{
    public partial class VD1_Form : Form
    {
        private static Random random = new Random();
        private static byte counter = 0;

        public VD1_Form()
        {
            InitializeComponent();
        }

        private void Start_btn_Click(object sender, EventArgs e)
        {
            RSAFileHelper rsa = new RSAFileHelper();

            //1
            LogMessage("Alice -> Bob: Levelezni szeretnék");
            TaskComplete(++counter);

            //2
            string pubKey = rsa.GetPubKey();
            LogMessage("Bob -> Alice: PUB_KEY: " + pubKey);
            TaskComplete(++counter);

            //3
            string aesKey = RandomString(32); //must b 32
            string chp = rsa.EncryptString(aesKey, pubKey);
            LogMessage("Alice -> Bob: " + chp);
            TaskComplete(++counter);

            //4
            string privKey = rsa.GetPrivKey();
            string reverted_aesKey = rsa.DecryptString(chp, privKey);
            LogMessage("*Bob*: DECRYPTED AES KEY: " + reverted_aesKey);
            TaskComplete(++counter);

            //5
            string msg = "Hello";
            string enc_msg = Aes256CbcEncrypter.Encrypt(msg, reverted_aesKey);
            LogMessage("Bob -> Alice: " + enc_msg);
            TaskComplete(++counter);

            //6
            string dec_msg = Aes256CbcEncrypter.Decrypt(enc_msg, aesKey);
            LogMessage("*Alice*: " + dec_msg);
            TaskComplete(++counter);

            Start_btn.Enabled = false;
            Start_btn.Text = "Finished";
        }

        private void LogMessage(string Message)
        {
            textBox1.Text += Message;
            for (byte i = 0; i < 6; i++)
                textBox1.AppendText(Environment.NewLine);
        }
        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private void TaskComplete(byte TaskNo)
        {
            ShowProgress(TaskNo);
            WaitFor(500);
        }
        private void ShowProgress(byte Stage)
        {
            switch (Stage)
            {
                case 1:
                    label8.Visible = true;
                    break;
                case 2:
                    label9.Visible = true;
                    break;
                case 3:
                    label10.Visible = true;
                    break;
                case 4:
                    label11.Visible = true;
                    break;
                case 5:
                    label12.Visible = true;
                    break;
                case 6:
                    label13.Visible = true;
                    break;
                default:
                    break;
            }
        }
        public void WaitFor(int milliseconds)
        {
            var timer1 = new Timer();
            if (milliseconds < 1) return;

            timer1.Interval = milliseconds;
            timer1.Enabled = true;
            timer1.Start();

            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
            };

            while (timer1.Enabled)
            {
                Application.DoEvents();
            }
        }
    }
}

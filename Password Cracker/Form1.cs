using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;

namespace Password_Cracker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void log(string text)
        {
            new Thread(() =>
            {
                richTextBox1.Invoke(new MethodInvoker(delegate
                {
                    DateTime currentDateTime = DateTime.Now;
                    string formattedDateTime = currentDateTime.ToString("HH:mm:ss");
                    richTextBox1.Text += $"[{formattedDateTime}] " + text + "\n";
                    richTextBox1.ScrollToCaret();
                }));
            })
            { IsBackground = true }.Start();
        }

        public string[] hash_types = { "MD5", "SHA-1", "SHA-256", "SHA-512" };
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange(hash_types);
            comboBox1.Text = hash_types[0];
        }

        public string ComputeMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        public string ComputeSHA1Hash(string input)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha1.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        public string ComputeSHA256Hash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        public string ComputeSHA512Hash(string input)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha512.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
        public void runtime_log()
        {
            new Thread(() =>
            {
                int total_passwords = progressBar1.Maximum;
                int tried_passwords = progressBar1.Value;
                DateTimeOffset currentTime = DateTimeOffset.UtcNow;
                long cur_t = currentTime.ToUnixTimeSeconds();
                
                if(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - cur_t >= 0.01)
                {
                    cur_t = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    MessageBox.Show("hi");
                    log($"'{tried_passwords}/{total_passwords}' still working !");
                }
            })
            { IsBackground = true }.Start();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string input = textBox1.Text;
            string hash_selected = comboBox1.Text;
            string result = "";
            richTextBox1.Text = "";
            progressBar1.Value = 0;

            bool _ = button2.Text == "Passwords List";
            if (_)
            {
                MessageBox.Show("Passwords list not found", "Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }

            else
            {
                int retries = 0;
                //runtime_log();
                progressBar1.Maximum = File.ReadAllText(button2.Text).Split('\n').Length;
                foreach (string password in File.ReadAllText(button2.Text).Split('\n'))
                {
                    
                    
                    switch (hash_selected)
                    {
                        case "MD5": result = ComputeMD5Hash(password); break;
                        case "SHA-1": result = ComputeSHA1Hash(password); break;
                        case "SHA-256": result = ComputeSHA256Hash(password); break;
                        case "SHA-512": result = ComputeSHA512Hash(password); break;

                    };
                    string pass = "failed";
                    if(result == input)
                    {
                        pass = "success";
                        log($"Password Cracked Successfully | Password: '{password}' / {result.Substring(0, 5)} ...");
                        log($"Password found after {retries} Retries");
                        progressBar1.Value = File.ReadAllText(button2.Text).Split('\n').Length;
                        MessageBox.Show($"Password Found '{password}'", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        return;
                    }
                    progressBar1.Value++;
                    retries++;
                    //log($"Trying '{password}' / {result.Substring(0,5)} ... - {pass}");
                }
                MessageBox.Show("Finished!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
        public void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            if(open.ShowDialog() == DialogResult.OK)
            {
                button2.Text = open.FileName;
            }
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
    }
}

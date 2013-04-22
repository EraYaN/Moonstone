/*-
 * Copyright (c) 2012 Software Development Solutions, Inc.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR AND CONTRIBUTORS ``AS IS'' AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED.  IN NO EVENT SHALL THE AUTHOR OR CONTRIBUTORS BE LIABLE
 * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS
 * OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
 * OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
 * SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Jamcast.Plugins.Spotify {

    public partial class LoginForm : Form {

        public string Username { get; private set; }
        public string Password { get; private set; }
        public string KeyFilePath { get; private set; }

        public LoginForm() {

            InitializeComponent();

            txtUsername.Text = Configuration.Instance.Username;

        }

        private void cmdLoadKey_Click(object sender, EventArgs e) {

            openFileDialog1.Multiselect = false;
            openFileDialog1.Title = "Open Spotify API Application Key";
            openFileDialog1.Filter = "Spotify Application Key (*.key) | *.key";
            openFileDialog1.ShowDialog(this);

            //TODO: basic validation of selected application key file

            this.KeyFilePath = openFileDialog1.FileName;
            lblFilename.Text = "OK!";            
            lblFilename.Visible = true;
            cmdLoadKey.Visible = false;

        }

        private void cmdOK_Click(object sender, EventArgs e) {

            string username, password;

            username = txtUsername.Text.Trim();
            password = txtPassword.Text.Trim();

            if (String.IsNullOrEmpty(username)
                || String.IsNullOrEmpty(password)) {

                MessageBox.Show("Please enter a valid username and password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            this.Username = username;
            this.Password = password;
            this.KeyFilePath = openFileDialog1.FileName;

            this.DialogResult = DialogResult.OK;
            this.Close();

        }

        private void cmdStartService_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {

            Process.Start("https://developer.spotify.com/en/libspotify/application-key/");

        }       

    }

}

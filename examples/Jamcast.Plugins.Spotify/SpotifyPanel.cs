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
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;

using Jamcast.Extensibility;

namespace Jamcast.Plugins.Spotify {

    public partial class SpotifyPanel : ConfigurationPanel {
        
        public SpotifyPanel() {

            InitializeComponent();
                                                
        }

        public override string DisplayName {

            get { return "Spotify"; }

        }

        protected override void OnServiceAvailable() {

            refresh();

        }

        private void refresh() {

            lblStatus.Text = String.Format("The Spotify plugin is {0}.", Configuration.Instance.IsEnabled ? "enabled" : "disabled");
            cmdEnable.Text = Configuration.Instance.IsEnabled ? "Disable" : "Enable";

        }
            
        private void cmdEnable_Click(object sender, EventArgs e) {

            if (Configuration.Instance.IsEnabled) {

                if (MessageBox.Show("Spotify content will no longer be available.  Are you sure you want to continue?", "Confirm Disable Plugin", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                Configuration.Instance.IsEnabled = false;
                Configuration.Instance.Password = null;
                Configuration.Instance.ApplicationKey = null;

            } else {

                LoginForm frm = new LoginForm();

                if (frm.ShowDialog(this) != DialogResult.OK)
                    return;

                Configuration.Instance.ApplicationKey = File.ReadAllBytes(frm.KeyFilePath);
                Configuration.Instance.Username = frm.Username;
                Configuration.Instance.Password = frm.Password;
                Configuration.Instance.IsEnabled = true;
                
            }

            Configuration.Instance.Save();

            refresh();

            this.RequestRestart();

        }

    }

}

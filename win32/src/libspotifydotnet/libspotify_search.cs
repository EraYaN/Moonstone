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
using System.Runtime.InteropServices;

namespace libspotifydotnet {

    public delegate void search_complete_cb_delegate(IntPtr searchPtr, IntPtr userDataPtr);

    public enum sp_search_type {
        SP_SEARCH_STANDARD = 0,
        SP_SEARCH_SUGGEST = 1,
    }

    public static partial class libspotify {

        public enum sp_radio_genre {
            ALT_POP_ROCK = 0x1,
            BLUES = 0x2,
            COUNTRY = 0x4,
            DISCO = 0x8,
            FUNK = 0x10,
            HARD_ROCK = 0x20,
            HEAVY_METAL = 0x40,
            RAP = 0x80,
            HOUSE = 0x100,
            JAZZ = 0x200,
            NEW_WAVE = 0x400,
            RNB = 0x800,
            POP = 0x1000,
            PUNK = 0x2000,
            REGGAE = 0x4000,
            POP_ROCK = 0x8000,
            SOUL = 0x10000,
            TECHNO = 0x20000
        }

        [DllImport("libspotify")]
        public static extern IntPtr sp_search_create(IntPtr sessionPtr, string query, int track_offset, int track_count,
                                                       int album_offset, int album_count, int artist_offset, int artist_count,
                                                       int playlist_offset, int playlist_count, sp_search_type search_type,
                                                       IntPtr callbackPtr, IntPtr userDataPtr);

        [DllImport("libspotify")]
        public static extern IntPtr sp_radio_search_create(IntPtr sessionPtr, int from_year, int to_year, sp_radio_genre genres,
                                                        IntPtr callbackPtr, IntPtr userDataPtr);

        [DllImport("libspotify")]
        public static extern bool sp_search_is_loaded(IntPtr searchPtr);

        [DllImport("libspotify")]
        public static extern sp_error sp_search_error(IntPtr searchPtr);

        [DllImport("libspotify")]
        public static extern int sp_search_num_tracks(IntPtr searchPtr);

        [DllImport("libspotify")]
        public static extern IntPtr sp_search_track(IntPtr searchPtr, int index);

        [DllImport("libspotify")]
        public static extern int sp_search_num_albums(IntPtr searchPtr);

        [DllImport("libspotify")]
        public static extern IntPtr sp_search_album(IntPtr searchPtr, int index);

        [DllImport("libspotify")]
        public static extern int sp_search_num_artists(IntPtr searchPtr);

        [DllImport("libspotify")]
        public static extern IntPtr sp_search_artist(IntPtr searchPtr, int index);

        [DllImport("libspotify")]
        public static extern IntPtr sp_search_query(IntPtr searchPtr);

        [DllImport("libspotify")]
        public static extern IntPtr sp_search_did_you_mean(IntPtr searchPtr);

        [DllImport("libspotify")]
        public static extern int sp_search_total_tracks(IntPtr searchPtr);

        [DllImport("libspotify")]
        public static extern int sp_search_total_albums(IntPtr searchPtr);

        [DllImport("libspotify")]
        public static extern int sp_search_total_artists(IntPtr searchPtr);

        [DllImport("libspotify")]
        public static extern sp_error sp_search_add_ref(IntPtr searchPtr);

        [DllImport("libspotify")]
        public static extern sp_error sp_search_release(IntPtr searchPtr);
        
    }

}

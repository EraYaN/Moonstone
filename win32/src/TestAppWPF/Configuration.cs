﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TestAppWPF
{
	static internal class Configuration
	{
		internal static byte[] appkey = {
	        0x01, 0x1F, 0xF2, 0xFE, 0xD3, 0xC6, 0x9A, 0x81, 0x43, 0x55, 0xDF, 0x3B, 0x99, 0xA1, 0x24, 0x16,
	        0x87, 0x95, 0xE2, 0xE3, 0xAC, 0x2D, 0x43, 0x6D, 0xC7, 0x0B, 0x7A, 0x91, 0x7C, 0x87, 0x83, 0xCE,
	        0xEA, 0xCA, 0x6C, 0xE1, 0x6B, 0x2D, 0x5D, 0x4B, 0xD1, 0x28, 0x40, 0xC0, 0xA4, 0xD7, 0xF4, 0xC3,
	        0xBA, 0x3A, 0x94, 0x7C, 0x69, 0x3A, 0xC7, 0x95, 0x49, 0x1F, 0x84, 0xC9, 0x69, 0x82, 0x15, 0xD5,
	        0xA6, 0x37, 0x0F, 0xBF, 0x82, 0x4D, 0x61, 0xDF, 0x3C, 0x60, 0xDC, 0xD7, 0x87, 0x91, 0x9D, 0xAE,
	        0xE8, 0x07, 0x1D, 0xDC, 0x73, 0xC0, 0xDD, 0xBF, 0xB0, 0x38, 0x50, 0x64, 0x53, 0x0C, 0xEA, 0x6A,
	        0x37, 0x2B, 0x56, 0x7D, 0xC1, 0xFF, 0x13, 0x5F, 0xD1, 0x65, 0xA4, 0xEA, 0x85, 0xF5, 0xD4, 0x13,
	        0x79, 0x42, 0xEC, 0xDD, 0x33, 0xBF, 0x27, 0x39, 0xE5, 0xDD, 0xDE, 0xA2, 0x37, 0xBD, 0xEC, 0x01,
	        0x78, 0x7F, 0x53, 0x1F, 0xA5, 0x55, 0xAA, 0xD8, 0x14, 0x55, 0x87, 0xB8, 0x24, 0x8D, 0x1D, 0x8A,
	        0x91, 0x45, 0x45, 0x6B, 0x34, 0x12, 0x01, 0x8C, 0xBA, 0x39, 0x2F, 0xED, 0x96, 0x36, 0xB8, 0x51,
	        0x29, 0x75, 0x11, 0x1C, 0x2A, 0x16, 0x76, 0xCF, 0xEB, 0x8A, 0xA8, 0x64, 0xC9, 0xFD, 0x61, 0x76,
	        0x61, 0x11, 0xA7, 0x7D, 0x29, 0x7C, 0x6B, 0x08, 0xFB, 0x14, 0xE1, 0xD3, 0x44, 0x3D, 0x6D, 0x45,
	        0x9A, 0x31, 0xB6, 0x7E, 0x83, 0x5D, 0x31, 0x56, 0xD4, 0x53, 0x71, 0xC3, 0x34, 0xE1, 0x19, 0x2D,
	        0xA2, 0xBF, 0x8A, 0xCA, 0x35, 0x65, 0xBB, 0xC4, 0x05, 0x3B, 0x3A, 0xCC, 0xF3, 0x7D, 0x08, 0x38,
	        0xB6, 0x62, 0xB3, 0x05, 0x4B, 0xF4, 0x41, 0xC8, 0x80, 0x17, 0x13, 0x4A, 0x85, 0x3F, 0x4D, 0x93,
	        0x41, 0x71, 0x64, 0xB2, 0xDC, 0xAD, 0xEB, 0x11, 0x1A, 0x17, 0xB3, 0xDC, 0xBB, 0x8C, 0x6C, 0x43,
	        0xD9, 0x05, 0x45, 0xD7, 0x23, 0x8F, 0xEE, 0x80, 0xE1, 0x90, 0xD3, 0x5B, 0x51, 0x1E, 0x9A, 0x9A,
	        0x22, 0x4E, 0x10, 0xF1, 0xAF, 0x51, 0x1A, 0x50, 0x87, 0xE2, 0xFB, 0x22, 0x54, 0x2F, 0x19, 0xD9,
	        0xAD, 0x7C, 0x2B, 0xD7, 0x29, 0x77, 0x2A, 0xB0, 0x20, 0xB0, 0x2F, 0xA7, 0x35, 0x4B, 0x75, 0x0A,
	        0x6D, 0x1D, 0xC2, 0x9E, 0xD7, 0xB0, 0x55, 0x6D, 0x44, 0x3B, 0x75, 0x03, 0x4B, 0x71, 0x72, 0xCA,
	        0x4E,
        };
        internal const string CLIENT_NAME = "TestAppWPF";
        internal static string cache = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), CLIENT_NAME, "cache");
        internal static string settings = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), CLIENT_NAME, "settings");
        internal static string userAgent = CLIENT_NAME;
        
	}
}

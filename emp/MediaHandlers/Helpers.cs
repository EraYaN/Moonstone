using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMP
{
	/// <summary>
	/// The audio codec of a certain file.
	/// </summary>
	public enum AudioCodec
	{
		/// <summary>
		/// Unknown Audio Codec
		/// </summary>
		Unknown,
		/// <summary>
		/// MPEG-1/2 Layer III Audio Codec
		/// </summary>
		MP3,
		/// <summary>
		/// Advanced Audio Coding Audio Codec
		/// </summary>
		AAC,
		/// <summary>
		/// Digital Theatre System Coherent Acoustics Audio Codec
		/// </summary>
		DTS,
		/// <summary>
		/// Dolby Digital Audio Codec (A/52)
		/// </summary>
		AC3,
		/// <summary>
		/// Digital Theatre System Coherent Acoustics High Definition Audio Codec
		/// </summary>
		DTSHD,
		/// <summary>
		/// Windows Media Audio Audio Codec
		/// </summary>
		WMA,
		/// <summary>
		/// Microsoft "WAVE" Audio Codec
		/// </summary>
		Wave,
		/// <summary>
		/// Free Lossless Audio Codec
		/// </summary>
		FLAC,
		/// <summary>
		/// Apple Lossless Audio Codec
		/// </summary>
		ALAC,
		/// <summary>
		/// Dolby TrueHD Audio Codec
		/// </summary>
		TrueHD,
		/// <summary>
		/// Monkey's Audio Codec
		/// </summary>
		APE,
		/// <summary>
		/// RealAudio Audio Codec
		/// </summary>
		RealAudio,
		/// <summary>
		/// MPEG-1/2 Layer I Audio Codec
		/// </summary>
		MP1,
		/// <summary>
		/// MPEG-1/2 Layer II Audio Codec
		/// </summary>
		MP2,
		/// <summary>
		/// High-Efficiency Advanced Audio Coding Audio Codec
		/// </summary>
		HEAAC,
		/// <summary>
		/// Ogg Vorbis Audio Codec
		/// </summary>
		Vorbis,
		/// <summary>
		/// Generic Lossless Video Codec
		/// </summary>
		Lossless
	}
	/// <summary>
	/// The source where the video stream came form for a certain file.
	/// </summary>
	public enum VideoSource
	{
		/// <summary>
		/// Unknown Video Source
		/// </summary>
		Unknown,
		/// <summary>
		/// A copy made in a cinema using a camcorder or mobile phone. The sound source is the camera microphone. 
		/// </summary>
		Cam,
		/// <summary>
		/// A copy was shot in an empty cinema or from the projection booth with a professional camera mounted on a tripod, directly connected to the sound source.
		/// </summary>
		Telesync,
		/// <summary>
		/// A copy made from an unfinished version of a film produced by the studio.
		/// </summary>
		Workprint,
		/// <summary>
		/// A copy captured from a film print using a machine that transfers the movie from its analog reel to digital format.
		/// </summary>
		Telecine,
		/// <summary>
		/// PPVRips come from Pay-Per-View sources, all the PPVRip releases are brand new movies which have not yet been released to Screener or DVD but are available for viewing by Hotel customers.
		/// </summary>
		PayPerView,
		/// <summary>
		/// These are early DVD or BD releases of the theatrical version of a film, typically sent to movie reviewers, Academy members, and executives for review purposes.
		/// </summary>
		Screener,
		/// <summary>
		/// DDC is basically the same as a Screener, but sent digitally (ftp/http/etc.) to companies instead of via the postal system.
		/// </summary>
		DDC,
		/// <summary>
		///RX releases differ from normal releases in that they are a direct Telecine transfer of the film without any of the image processing.    R0 No Region Coding
		///R1 United States of America, Canada
		///R2 Europe, including France, Greece, Turkey, Egypt, Arabia, Japan and South Africa
		///R3 Korea, Thailand, Vietnam, Borneo and Indonesia
		///R4 Australia and New Zealand, Mexico, the Caribbean, and South America
		///R5 India, Africa, Russia and former USSR countries
		///R6 Peoples Republic of China
		///R7 Unused
		///R8 Airlines/Cruise Ships
		///R9 Expansion (often used as region free)
		///
		///R1 and R2 considered best quality.
		/// </summary>
		RX,
		/// <summary>
		/// A final retail version of a film. Transcoded into a digital container format from the source, a DVD.
		/// </summary>
		DVDRip,
		/// <summary>
		/// A final retail version of a film in DVD format, generally a complete copy from the original DVD.
		/// </summary>
		DVDR,
		/// <summary>
		/// TVRip is a capture source from an analog capture card (coaxial/composite/s-video connection)
		/// Digital satellite rip (DSR) is a rip that is captured from a non standard definition digital source like satellite.
		/// HDTV or PDTV or DTH (Direct To Home) rips often come from Over-the-Air transmissions and are captured with a PVR.
		/// </summary>
		TVRip,		
		/// <summary>
		/// VODRip stands for Video-On-Demand Rip. This can be done by recording or capturing a video/movie from an On-Demand service such as through a cable or satellite TV service.
		/// </summary>
		VODRip,
		/// <summary>
		/// A final retail version of a film. Transcoded into a digital container format from the source, a BluRay.
		/// </summary>
		BluRayRip,
		/// <summary>
		/// A final retail version of a film in BluRay format, generally a complete copy from the original BluRay. Often called BD25 or BD50
		/// </summary>
		BluRay
	}
	/// <summary>
	/// The quality of the video stream for a certain file.
	/// </summary>
	public enum VideoQuality
	{
		/// <summary>
		/// Unknown Video Quality
		/// </summary>
		Unknown,
		/// <summary>
		/// 1080p or 1080i. The latter is less common.
		/// </summary>
		FullHD,
		/// <summary>
		/// 720p or 720i. The latter is less common.
		/// </summary>
		HDReady,
		/// <summary>
		/// Phase Alternating Line Analog Television Format. Used in Europe (except France), Asia-Pacific (except Japan), parts of Africa.
		/// </summary>
		PAL,
		/// <summary>
		/// National Television System Committee Analog Television Format. Used in the US, Canada and parts of South-America.
		/// </summary>
		NTSC,
		/// <summary>
		/// Séquentiel couleur à mémoire or Sequential Color with Memory Analog Television Format. Used in France, Former Sovjet Union, parts of Africa.
		/// </summary>
		SECAM,
		/// <summary>
		/// Standard difinition. Like 480p, 360p, 240p.
		/// </summary>
		SD
	}
	/// <summary>
	/// The video codec used when encoding a certain file.
	/// </summary>
	public enum VideoCodec
	{
		/// <summary>
		/// Unknown Video Codec
		/// </summary>
		Unknown,
		/// <summary>
		/// H.264/MPEG-4 Part 10 or AVC Video Codec
		/// </summary>
		H264,
		/// <summary>
		/// DivX Pro Codec Video Codec. MPEG-4 ASP.
		/// </summary>
		DivX,
		/// <summary>
		/// Xvid Video Codec. MPEG-4 ASP. Based on OpenDivX.
		/// </summary>
		Xvid,
		/// <summary>
		/// FFmpeg MPEG-4 Video Codec
		/// </summary>
		MPEG4,
		/// <summary>
		/// Nero Digital Video Codec. MPEG-4 ASP or AVC
		/// </summary>
		Nero,
		/// <summary>
		/// Sorenson 3/Spark Video Codec
		/// </summary>
		QuickTime,
		/// <summary>
		/// Windows Media Video 7/8/9 Codec
		/// </summary>
		WMV,
		/// <summary>
		/// On2 Technologies TrueMotion VP3/VP4, VP5, VP6, VP7, VP8 Video Codecs
		/// </summary>
		VPX,
		/// <summary>
		/// Theora Video Codec. Developed by the Xiph.Org Foundation
		/// </summary>
		Theora,
		/// <summary>
		/// RealVideo Video Codec. Developer by RealNetworks
		/// </summary>
		RealVideo,
		/// <summary>
		/// Schrödinger dirac-research Video Codec
		/// </summary>
		Dirac,
		/// <summary>
		/// Indeo Video Codec. Developed by Intel
		/// </summary>
		Indeo,
		/// <summary>
		/// Cinepak Video Codec. Old Apple QuickTime codec.
		/// </summary>
		Cinepak,
		/// <summary>
		/// DVC Video Codec. Used by a lot of video camera's
		/// </summary>
		DV,
		/// <summary>
		/// Generic Lossless Video Codec
		/// </summary>
		Lossless
	}
	/// <summary>
	/// The used container format for a certain file.
	/// </summary>
	public enum Container
	{
		/// <summary>
		/// Unknown Digital Container Format
		/// </summary>
		Unknown,
		/// <summary>
		/// Audio Video Interleaved Digital Container Format (AVI). Based on RIFF.
		/// </summary>
		AVI,
		/// <summary>
		/// QuickTime Digital Container Format (MOV). Developed by Apple Inc.
		/// </summary>
		QuickTime,
		/// <summary>
		/// Matroska Multimedia Container Digital Format (MKV/MK3D/MKA/MKS). Developed by Matroska.org. Completely open specification. Can hold nearly anything.
		/// </summary>
		Matroska,
		/// <summary>
		/// MPEG-4 Part 14 or MP4 (formally ISO/IEC 14496-14:2003) Digital Container Format (MP4/M4A/M4V).
		/// </summary>
		MPEG4,
		/// <summary>
		/// Flash Video Digital Container Format (FLV/F4V/F4P)
		/// </summary>
		Flash,
		/// <summary>
		/// Interchange File Format Digital Container Format (IFF). Developed by Electronic Arts and Commodore-Amiga.
		/// </summary>
		IFF,
		/// <summary>
		/// MPEG program stream (MPG/MPEG/PS)
		/// </summary>
		MPEGPS,
		/// <summary>
		/// MPEG transport stream (MTS/TS)
		/// </summary>
		MPEGTS,
		/// <summary>
		/// Ogg Digital Container Format (OGV/OGA/OGX/OGG/SPX). Developed by Xiph.Org Foundation.
		/// </summary>
		Ogg,
		/// <summary>
		/// WebM Digital Container Format (WEBM). Developed by On2, Xiph, Matroska and Google.
		/// </summary>
		WebM,
		/// <summary>
		/// RealMedia Digital Container Format (RM). Developed by RealNetworks.
		/// </summary>
		RM
	}
}

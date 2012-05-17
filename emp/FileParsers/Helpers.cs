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
		Vorbis			
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
		/// HDTV or PDTV or DTH (Direct To Home) rips often come from Over-the-Air transmissions and are captured with a HD PVR.
		/// </summary>
		HDTV,
		/// <summary>
		/// SDTV is a capture source from an analog capture card (coaxial/composite/s-video connection) standard definition TV and are captured with a PVR.
		/// </summary>
		SDTV,
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
		Unknown,
		H264,
		DivX,
		Xvid,
		MPEG4,
		Nero,
		QuickTime,
		WMV,
		VPX,
		Theora,
		RealVideo,
		Dirac,
		Indeo,
		Cinepak,
		DV,
		Lossless
	}
	/// <summary>
	/// The used container format for a certain file.
	/// </summary>
	public enum Container
	{
		Unknown,
		AVI,
		QuickTime,
		Matroska,
		MPEG4,
		Flash,
		IFF,
		MPEGP,
		MPEGTS,
		Ogg,
		WebM,
		RM
	}
}

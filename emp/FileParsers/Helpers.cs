using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMP
{
	public enum AudioCodec
	{
		Unknown,
		MP3,
		AAC,
		DTS,
		AC3,
		DTSHD,
		WMA,
		Wave,
		FLAC,
		ALAC,
		DolbyTrueHD,
		Monkey,
		RealAudio,
		MP1,
		MP2,
		HEAAC,
		Vorbis,
		WMA
	}
	public enum VideoSource
	{
		Unknown,
		Cam,
		Telesync,
		Workprint,
		Telecine,
		PayPerView,
		Screener,
		DigitalDistributionCopy,
		RX,
		DVDRip,
		DVDR,
		HDTV,
		SDTV,
		VODRip,
		BluRayRip,
		BluRay
	}
	public enum VideoQuality
	{
		Unknown,
		FullHD,
		HDReady,
		PAL,
		NTSC,
		SD
	}
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

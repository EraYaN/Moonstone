using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMP
{
	public static class HelperExtensionMethods
	{
		//TODO make shit out of this
		public static String ToDisplayString(this Container container)
		{
			return container.ToString();
		}
		public static String ToDisplayString(this AudioCodec audiocodec)
		{
			return audiocodec.ToString();
		}
		public static String ToDisplayString(this VideoCodec videocodec)
		{
			return videocodec.ToString();
		}
		public static String ToDisplayString(this VideoQuality videoquality)
		{
			return videoquality.ToString();
		}
		public static String ToDisplayString(this VideoSource videosource)
		{
			return videosource.ToString();
		}
	}
}

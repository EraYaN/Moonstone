#include <appkey.h>
#include <stdio.h>
#include <stdlib.h>
#include <libspotify\api.h>

static sp_session_callbacks session_callbacks = {
    .logged_in = &on_login,
    .notify_main_thread = &on_main_thread_notified,
    .music_delivery = &on_music_delivered,
    .log_message = &on_log,
    .end_of_track = &on_end_of_track
};
 
static sp_session_config spconfig = {
    .api_version = SPOTIFY_API_VERSION,
    .cache_location = "tmp",
    .settings_location = "tmp",
    .application_key = g_appkey,
    .application_key_size = 0, // set in main()
    .user_agent = "spot",
    .callbacks = &session_callbacks,
    NULL
};

int main(){

	printf("Blaaa");
	getchar();
	return 0;
}
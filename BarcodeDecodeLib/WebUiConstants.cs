namespace BarcodeDecodeLib;

public static class WebUiConstants
{
    public const string USER_ID = "user_id";
    public const string USER_NAME = "user_name";
    public const string USER_TOKEN = "user_token";

    public const string USER_ACCESS_LEVEL_PROGER = "service";
    public const string USER_ACCESS_LEVEL_ADMIN = "admin";
    public const string USER_ACCESS_LEVEL_USER = "user";

    public const string USER_HAVE_NOT_ACCES_TO_CONTROL = "User does not have access to control";
    public const string PASSWORD_OR_NAME_IS_INCORRECT = "Password or name is incorrect";

    public const int PAGINATION_SIZE = 20;

    public const int TIME_SETTING_UTC = 3;

    public const string USER_LAST_PAGE = "last_page";

    public const string USER_HOURS_OFFSET = "Hour Offset";

    public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

    //FOR LOG CONSTANT
    public const string LOG_USER = "user";
    public const string LOG_METHOD_NAME = "method.name";
    public const string LOG_METHOD_DURATION = "method.duration";
    public const string LOG_PASSWORD = "password";
    public const string LOG_EMAIL = "email";
    public const string LOG_SETTING = "setting";
    public const string LOG_USER_CHANGE_SORT_METHOD_NAME = "User changed sort method name";
    public const string LOG_USER_CHANGE_CHANNEL_PROPERTY = "User changed channel property";
    public const string LOG_USER_CHANGE_SETTING = "User change setting";
    public const string LOG_USER_SUCCESS_LOGIN = "User success login";
    public const string LOG_USER_FAIL_LOGIN = "User fail login";
    public const string LOG_USER_LOGOUT = "User logout";
    public const string LOG_USER_REGISTER = "User register";
}
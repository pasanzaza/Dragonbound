<?php
//ini_set('display_errors', 1);
error_reporting(E_ALL);
require_once("fbsdk/facebook.php");


$config = array();
$config['appId'] = '1397938667086072';
$config['secret'] = 'xdddd';
$config['fileUpload'] = false;
  
$facebook = new Facebook($config);

$user = $facebook->getUser();
$user_profile = array();
$logoutUrl = "";
$loginUrl = "";

if ($user) {
  try {
    $user_profile = $facebook->api('/me');
  } catch (FacebookApiException $e) {
    error_log("[fbsdk] ".$e);
    $user = null;
  }
}

if ($user) {
  $logoutUrl = $facebook->getLogoutUrl();
} else {
  $loginUrl = $facebook->getLoginUrl();
}

if ($user)
{
    echo '[146634,0,0,"4dcde013e8121f7cee5e","PE"]';
}
else
{
    echo "[0]";
}
?>
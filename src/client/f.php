<?php
//ini_set('display_errors', 1);
error_reporting(E_ALL);
$t = @$_POST["t"];
$u = @$_POST["u"];
$p = @$_POST["p"];

if ($t == "1")
{
    require_once("fbsdk/facebook.php");
    
    $config = array();
    $config['appId'] = '1397938667086072';
    $config['secret'] = 'xd';
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
    
    echo '[146634,0,0,"955fa89ea0e762fbe7c4","PE"]';
}
else  if($t == "2")
{
    //buscar el usuario, si existe sacar el password + secure code
    
    $_p = "carlosx";         //password
    $_s = "|" . "123456";    //secure code
    
    $_f = hash('sha256', $_p.$_s, true);
    
    $_c = strhex($_f);
    //950e90b558019a923826d889bc7a42cab72aff82a65171cbec983a9aafc7f496
    if ($_c == $p)
    {
        echo '[146634,0,0,"955fa89ea0e762fbe7c4","PE"]';
    }
    else
    {
        echo '[22,"Your username or password were incorrect. Try again."]';
    }
}
function hexstr($hexstr) {
  $hexstr = str_replace(' ', '', $hexstr);
  $hexstr = str_replace('\x', '', $hexstr);
  $retstr = pack('H*', $hexstr);
  return $retstr;
}

function strhex($string) {
  $hexstr = unpack('H*', $string);
  return array_shift($hexstr);
}
?>
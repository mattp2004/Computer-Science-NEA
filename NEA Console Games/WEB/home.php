<?php
$website = file_get_contents('http://38.242.132.154:9000/api/uuid?token=hello');
session_start();
if(!isset($_SESSION['loggedin'])){
    header('Location: index.html');
    exit;
}

function ping($host, $port, $timeout) 
{ 
  $tB = microtime(true); 
  $fP = fSockOpen($host, $port, $errno, $errstr, $timeout); 
  if (!$fP) { return "Offline 🔴 "; } 
  $tA = microtime(true); 
  return round((($tA - $tB) * 1000), 0)." ms 🟢"; 
}

$pingAPI = ping('38.242.132.154', '443', '1000');
$pingGS = ping('38.242.132.154', '6000', '1000');
$pingWEB = ping('38.242.132.154', '80', '1000');
?>
<!DOCTYPE html>
<html>
	<head>
		<meta charset="utf-8">
		<title>Home Page</title>
        <link href="style.css" rel="stylesheet" type="text/css">
		<link rel="stylesheet" href="https://use.fontawesome.com/releases/v5.7.1/css/all.css">
	</head>
	<body class="loggedin">
		<nav class="navtop">
			<div>
				<h1>GameServer Portal</h1>
				<a href="lookup.php"><i class="fas fa-search"></i>Lookup</a>
				<a href="profile.php"><i class="fas fa-user-circle"></i>Profile</a>
				<a href="logout.php"><i class="fas fa-sign-out-alt"></i>Logout</a>
			</div>
		</nav>
		<div class="content">
			<h2>Home Page</h2>
			<p>Welcome back, <?=$_SESSION['name']?></p>
		</div>
        <div class="content">
          <h2>
            Status
          </h2>
          <p>
            <b>GameServer</b>: <?=$pingGS?><br>
            <b>API</b>: <?=$pingAPI?><br>
            <b>Website</b>: <?=$pingWEB?><br> 
          </p>
      </div>
		<div class="content">
			<h2>Servers</h2>
          <p>
            NAME: GameServer-1<br>
            ID: 1<br>
            CREATED: 12/2/2022 4:a03:29 PM<br>
            LAST PING: 12/4/2022 6:28:53 PM<br>
            PORT: 6000<br>
          </p>
          <p><?=$website?></p>
		</div>
	</body>
</html>
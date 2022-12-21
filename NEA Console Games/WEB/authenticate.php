<?php
session_start();
$DATABASE_HOST = 'localhost';
$DATABASE_USER = 'a';
$DATABASE_PASS = 'a';
$DATABASE_NAME = 'data';
$con = mysqli_connect($DATABASE_HOST, $DATABASE_USER, $DATABASE_PASS, $DATABASE_NAME);
if (mysqli_connect_errno()) {
	exit('Failed to connect to MySQL: ' . mysqli_connect_error());
}
if (!isset($_POST['username'], $_POST['password'])) {
    echo '<script>alert("Access denied.")</script>';
}
if ($stmt = $con->prepare('SELECT id, password FROM accounts WHERE username = ?')) {
	$stmt->bind_param('s', $_POST['username']);
	$stmt->execute();
	$stmt->store_result();

    if($stmt->num_rows > 0){
        $stmt->bind_result($id, $password);
        $stmt->fetch();
        //if(password_verify($_POST['password'], $password)){ 
        if ($_POST['password'] === $password) {
            session_regenerate_id();
            $_SESSION['loggedin'] = TRUE;
            $_SESSION['name'] = $_POST['username'];
            $_SESSION['id'] = $id; 
            header('Location: home.php');
        }
        if(password_verify('password', $password)){
            header('Location: changepassword.php');
        }
        else{
            echo '<script>alert("Incorrect username or password")</script>';
        }
    }
    else{
        echo '<script>alert("Incorrect username or password")</script>';
    }
    

	$stmt->close();
}
?>
extends CanvasLayer

@onready var login_button: Button = %GoogleLoginButton

func _ready():
    login_button.pressed.connect(_on_login_button_pressed)


func _on_login_button_pressed():
    OS.shell_open("http://localhost:8000/auth/google/login")

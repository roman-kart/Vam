# Vam
Простой консольный текстовый редактор для Windows
## Установка
### Without git (Windows, CMD)
1. Откройте командную строку,
2. Перейдите в папку, в которую хотите скачать программу,
3. Скопируйте код, приведенный ниже и вставьте его в командную строку:
```
mkdir Vam
cd /D Vam  
powershell -Command "(New-Object Net.WebClient).DownloadFile('https://github.com//roman-kart/Vam/archive/refs/heads/main.zip', 'VamAlpha0.zip')" 
powershell -command "Expand-Archive -Force 'VamAlpha0.zip' 'VamAlpha0'"
set vamCurrentSession=%cd%\Vam\VamAlpha0\Vam-main\Vam\bin\Debug\Vam.exe  
.\VamAlpha0\Vam-main\Vam\bin\Debug\Vam.exe --help 
echo To use Vam in current session write ^%vamCurrentSession^% and press Enter. If you want to start Vam by writing "vam", you have to add [yourPath]\Vam\VamAlpha0\Vam-main\Vam\bin\Debug\ to the Path - environment variable.
```
## Использование
### Редактирование текста
```
Vam.exe --vam [pathToFile...]
```
### Вывод содержимого файла на экран
```
Vam.exe --cat [pathToFile...]
```
## Реализованные функции
- Tab
- Backspace 
- Enter
- Ввод текста при помощи клавиш
- Навигация при помощи стрелок

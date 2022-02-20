# Vam
Простой консольный текстовый редактор для Windows
## Установка
### Without git (Windows, CMD)
1. Откройте командную строку **(CMD.exe)**,
2. Перейдите в папку, в которую хотите скачать программу *(переход - cd ([подробнее](https://docs.microsoft.com/ru-ru/windows-server/administration/windows-commands/cd)), создать новую папку - mkdir ([подробнее](https://docs.microsoft.com/ru-ru/windows-server/administration/windows-commands/mkdir)))*,
3. Скопируйте код, приведенный ниже и вставьте его в командную строку:
```
mkdir Vam
cd /D Vam  
powershell -Command "(New-Object Net.WebClient).DownloadFile('https://github.com//roman-kart/Vam/archive/refs/heads/main.zip', 'VamAlpha0.zip')" 
powershell -command "Expand-Archive -Force 'VamAlpha0.zip' 'VamAlpha0'"
set vamCurrentSession=%cd%\VamAlpha0\Vam-main\Vam\bin\Debug\Vam.exe  
.\VamAlpha0\Vam-main\Vam\bin\Debug\Vam.exe --help 
echo To use Vam in current session write ^%vamCurrentSession^% and press Enter. If you want to start Vam by writing "vam", you have to add [yourPath]\Vam\VamAlpha0\Vam-main\Vam\bin\Debug\ to the Path - environment variable.
```
4. Для запуска программы в текущей сессии напишите в терминале
```
%vamCurrentSession% [args...]
```
Будьте внимательны, **после того, как вы закроете текущее окно командной строки, вы не сможете воспользоваться этой командой.**</br>
Для того, **чтобы запускать программу без указания пути**, 
**добавьте путь**, по которому расположена данная программа, **в переменную окружения Path**.
Подробнее в [официальной документации Microsoft](https://docs.microsoft.com/ru-ru/previous-versions/office/developer/sharepoint-2010/ee537574(v=office.14)).
## Использование
### Редактирование текста
```
Vam.exe --vam [pathToFile...]
```
**Если такого файла не существует - он создастся автоматически**
### Вывод содержимого файла на экран
```
Vam.exe --cat [pathToFile...]
```
### Управление
- **Ctrl + S** - **Сохранить** изменения в файле;
- **Ctrl + D** - **Выйти** из текстового редактора (**без сохранения изменений**);
- **Переменщение** - Блок стрелок: **← ↑ → ↓**;
## Реализованные функции
- Tab
- Backspace 
- Enter
- Ввод текста при помощи клавиш
- Навигация при помощи стрелок

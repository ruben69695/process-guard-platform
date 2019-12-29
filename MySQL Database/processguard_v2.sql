use processguarddb;
-- Principal entity
create table colors (
    name varchar(30) not null,
    description varchar(100) not null,
    
    primary key(name)
) engine = innodb;
insert into colors (name, description) values
	('black', 'Color que se utiliza para la lista negra de procesos'), ('white', 'Color que se utiliza para la lista blanca de procesos');
select * from colors;

-- Dependent entity
create table process_list (
    exe varchar(60),
    filename varchar(200),
    description varchar(150),
    color varchar(30), -- is the Foreign key
    
    primary key(exe, filename),
    
    foreign key (color)
		references colors(name)
) engine = innodb;

insert into process_list (exe, filename,description,color) values
	('Spotify','\\AppData\\Roaming\\Spotify\\Spotify.exe','Spotify app', 'black'), ('chrome','C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe','Navegador Google Chrome', 'black'), ('calc','C:\\Windows\\System32\\calc.exe','Calculadora oficial de Windows', 'white');
select * from process_list;

insert into process_list (exe, filename, description, color) values
	('Calculator','C:\\Program Files\\WindowsApps\\Microsoft.WindowsCalculator_10.1709.2703.0_x64__8wekyb3d8bbwe\\Calculator.exe', 'Calculadora de Windows 10', 'white');

insert into process_list (exe, filename, description, color) values
	('explorer','C:\\Windows\\explorer.exe', 'Explorador de Windows 10', 'black');
    
update process_list set filename = 'C:\\Windows\\System32\\calc.exe' where exe = 'Calculator';

insert into process_list (exe, filename, description, color) values
	('python', 'C:\\Program Files\\Python36\\python.exe', 'Linea de comandos para programar en Python', 'white');

insert into process_list (exe, filename, description, color) values
	('iexplore', 'C:\\Program Files\\internet explorer\\iexplore.exe', 'Internet explorer', 'black');
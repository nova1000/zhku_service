create table users (
    userID int key auto_increment,
    surname longtext, 
    name longtext,
    street longtext,
    house longtext,
    login longtext,
    password longtext
);
create table consuption (
    consID int key auto_increment,
    userID int,
    date longtext,
    value int,
    price int,
    foreign key (userID) references users (userID)
);
create table tariffs (
    tariffID int key auto_increment,
    name longtext,
    price int
);
insert into users (surname, name, street, house, login, password)
values("admin", "admin", "admin", "admin", "admin", "admin");
alter table consuption add namecons longtext;
insert into consuption (userID, date, value, price, namecons)
values(2, "Май", 500, 1500, "Газ");
insert into consuption (userID, date, value, price, namecons)
values(2, "Май", 200, 2000, "Электричество");
insert into consuption (userID, date, value, price, namecons)
values(3, "Июнь", 400, 1200, "Газ");
insert into consuption (userID, date, value, price, namecons)
values(3, "Июнь", 700, 7000, "Электричество");
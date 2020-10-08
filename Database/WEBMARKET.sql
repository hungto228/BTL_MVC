use DBMAKETMAKET;
create table admin( 
ad_id	int identity primary key,
ad_username nvarchar(50) not null unique,
ad_password nvarchar(50) not null,
)
insert into admin values('root','admin');
insert into admin values('admin','admin');
insert into admin values('test','admin');
select * from admin;
-------------------------
create table category(
cat_id int identity primary key,
cat_username nvarchar(50) not null unique,
cat_image nvarchar(max) not null ,
category_fk_admin int foreign key references admin (ad_id)
)
select *from category
------
create table users(
u_id int identity primary key,
u_username nvarchar(50) not null ,
u_email nvarchar(50) not null unique,
u_password nvarchar(50) not null ,
u_image nvarchar(max) not null ,
u_contact nvarchar(50) not null unique,

)
--------------------
create table product(
pro_id int identity primary key,
pro_username nvarchar(50) not null ,
pro_image nvarchar(max) not null ,
pro_desc nvarchar (max) not null,
pro_price int,
product_fk_cattegory int foreign key references category (cat_id),
product_fk_users int foreign key references users (u_id)
)
select*from product

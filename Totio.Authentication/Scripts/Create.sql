create table users
(
    id            text primary key,
    username      text not null unique,
    email         text not null unique,
    password_hash text not null,
    salt          text not null
);

insert into users
values ('D51A06D1-0553-42F9-84C1-732BD3D543F4',
        'user1',
        'email@gmail.com',
        'some-hash',
        'some-salt')

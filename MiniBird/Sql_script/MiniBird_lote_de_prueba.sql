-- Lotes iniciales de prueba
use MiniBird
go

-- Creando usuarios
insert into MiniBird.Person (UserName, Email, Password, NickName, PersonCryptID, RegistrationDate)
values ('@usuario', 'usuario@correo.com', 'asd123A', 'Usuario', '6b86b273ff34fce19d6b804eff5a3f5747ada4eaa22f1d49c01e52ddb7875b4b', GETDATE())

insert into MiniBird.Person (UserName, Email, Password, NickName, PersonCryptID, RegistrationDate)
values ('@invitado', 'invitado@correo.com', 'asd123A', 'Invitado', 'd4735e3a265e16eee03f59718b9b5d03019c07d8b6c51f90da3a666eec13ab35', GETDATE())

-- Creando posts
insert into MiniBird.Post (Comment, PublicationDate, ID_Person)
values ('Un #postInicial de demostración con url: https://www.google.com.ar en el mensaje', GETDATE(), 1)

insert into MiniBird.Post (Comment, PublicationDate, ID_Person)
values ('Con la cuenta invitado, un #postInicial de demostración con url: https://www.google.com.ar en el mensaje', GETDATE(), 2)

-- Creando réplicas a los post
insert into MiniBird.Post (Comment, PublicationDate, InReplyTo, ID_Person)
values ('Respondiendo al post del invitado.', GETDATE(), 2, 1)

insert into MiniBird.Post (Comment, PublicationDate, InReplyTo, ID_Person)
values ('Respondiendo al post del usuario.', GETDATE(), 1, 2)

-- Haciendo repost
insert into MiniBird.RePost (PublicationDate, ID_Post, ID_PersonThatRePost)
values (GETDATE(), 2, 1)

insert into MiniBird.RePost (PublicationDate, ID_Post, ID_PersonThatRePost)
values (GETDATE(), 3, 2)

-- Dando likes
insert into MiniBird.LikePost (DateOfAction, ID_Post, ID_PersonThatLikesPost)
values (GETDATE(), 4, 1)

insert into MiniBird.LikePost (DateOfAction, ID_Post, ID_PersonThatLikesPost)
values (GETDATE(), 1, 2)

-- Siguiendo al invitado
insert into MiniBird.Follow (ID_Person, ID_PersonFollowed, DateOfAction)
values (1 , 2, GETDATE())

-- Creando listas
insert into MiniBird.List (Name, Description, CreationDate, ID_Person)
values ('Lista dummy', 'Lista genérica', GETDATE(), 1)

insert into MiniBird.List (Name, Description, CreationDate, ID_Person)
values ('Segunda lista dummy', 'Segunda lista genérica', GETDATE(), 2)

-- Agregando usuario a la lista
insert into MiniBird.UserToList (ID_Person, ID_List, DateOfAggregate)
values (2, 1, GETDATE())

insert into MiniBird.UserToList (ID_Person, ID_List, DateOfAggregate)
values (1, 2, GETDATE())

-- Creando hashtag
insert into MiniBird.Hashtag (Name, UseCount, CreationDate)
values ('#postInicial', 2, GETDATE())

insert into MiniBird.PostToHashtag (ID_Post, ID_Hashtag)
values (1, 1)

insert into MiniBird.PostToHashtag (ID_Post, ID_Hashtag)
values (2, 1)

select * from MiniBird.Person
select * from MiniBird.Post
select * from MiniBird.Thumbnail
select * from MiniBird.RePost
select * from MiniBird.LikePost
select * from MiniBird.Follow
select * from MiniBird.List
select * from MiniBird.UserToList
select * from MiniBird.Hashtag
select * from MiniBird.PostToHashtag
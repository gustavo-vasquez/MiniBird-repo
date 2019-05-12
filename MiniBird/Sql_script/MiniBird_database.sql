create database MiniBird
go

use MiniBird
go

create schema MiniBird
go

create table MiniBird.Person (
	PersonID int identity(1,1),
	UserName nvarchar(16) not null,
	Email nvarchar(100) not null,
	Password nvarchar(max) not null,
	NickName nvarchar(50) not null,	
	PersonalDescription nvarchar(100) null,
	WebSiteURL nvarchar(100) null,
	Birthdate datetime null,
	ProfileAvatar varbinary(max) null,
	ProfileAvatar_MimeType nvarchar(15) null,
	ProfileHeader varbinary(max) null,
	ProfileHeader_MimeType nvarchar(15) null,
	DarkMode bit default 0,
	PersonCryptID nvarchar(max) null,	
	RegistrationDate datetime not null,
	constraint PKPerson primary key clustered (PersonID)
)
go

create table MiniBird.Post (
	PostID int identity(1,1),
	Comment nvarchar(280) not null,
	GIFImage nvarchar(max) null,
	VideoFile nvarchar(max) null,
	PublicationDate datetime not null,
	InReplyTo int null,
	ID_Person int not null,
	constraint PKPost primary key clustered (PostID),
	constraint FKPost_PostT foreign key (InReplyTo) references MiniBird.Post,
	constraint FKPost_PersonT foreign key (ID_Person) references MiniBird.Person (PersonID)
)
go

create table MiniBird.Thumbnail (
	ThumbnailID int identity(1,1),
	FilePath nvarchar(max) not null,
	ID_Post int not null,
	constraint PKThumbnailID primary key clustered (ThumbnailID),
	constraint FKThumbnailID_PostT foreign key (ID_Post) references MiniBird.Post (PostID)
)
go


create table MiniBird.RePost (
	RePostID int identity(1,1),	
	PublicationDate datetime not null,
	ID_Post int not null,
	ID_PersonThatRePost int not null,
	constraint PKRePost primary key clustered (RePostID),
	constraint FKRePost_PostT foreign key (ID_Post) references MiniBird.Post (PostID),
	constraint FKRePost_PersonT foreign key (ID_PersonThatRePost) references MiniBird.Person (PersonID)
)
go

create table MiniBird.LikePost (
	LikePostID int identity(1,1),
	DateOfAction datetime not null,
	ID_Post int not null,
	ID_PersonThatLikesPost int not null,	
	constraint PKLikePost primary key clustered (LikePostID),
	constraint FKLikePost_PostT foreign key (ID_Post) references MiniBird.Post (PostID),
	constraint FKLikePost_PersonT foreign key (ID_PersonThatLikesPost) references MiniBird.Person (PersonID)
)
go

create table MiniBird.Follow (
	ID_Person int not null,
	ID_PersonFollowed int not null,
	DateOfAction datetime not null,
	constraint PKFollow primary key (ID_Person, ID_PersonFollowed),
	constraint FKFollow_Follower_PersonT foreign key (ID_Person) references MiniBird.Person (PersonID),
	constraint FKFollow_Followed_PersonT foreign key (ID_PersonFollowed) references MiniBird.Person (PersonID)
)
go

create table MiniBird.List (
	ListID int identity(1,1),
	Name nvarchar(100) not null,
	Description nvarchar(100) null,
	IsPrivate bit default 0,
	CreationDate datetime not null,
	ID_Person int not null,
	constraint PKList primary key clustered (ListID),
	constraint FKList_PersonT foreign key (ID_Person) references MiniBird.Person (PersonID)
)
go

create table MiniBird.UserToList (
	ID_Person int not null,
	ID_List int not null,
	DateOfAggregate datetime not null,
	constraint PKUserToList primary key clustered (ID_Person, ID_List),
	constraint FKUserToList_PersonT foreign key (ID_Person) references MiniBird.Person (PersonID),
	constraint FKUserToList_ListT foreign key (ID_List) references MiniBird.List (ListID)
)
go

create table MiniBird.Hashtag (
	HashtagID int identity(1,1),
	Name nvarchar(30) not null,	
	UseCount int default 0,
	CreationDate datetime not null,	
	constraint PKHashtag primary key clustered (HashtagID)
)
go

create table MiniBird.PostToHashtag (
	ID_Post int not null,
	ID_Hashtag int not null,
	constraint PKPostToHashtag primary key clustered (ID_Post,ID_Hashtag),
	constraint FKPostToHashtag_PostT foreign key (ID_Post) references MiniBird.Post (PostID),
	constraint FKPostToHashtag_HashtagT foreign key (ID_Hashtag) references MiniBird.Hashtag (HashtagID)
)
go
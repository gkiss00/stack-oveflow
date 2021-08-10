DELETE FROM Votes
DELETE FROM Posttags
DELETE FROM Comments
DELETE FROM Posts
DELETE FROM Users

INSERT INTO [dbo].[Tags] ([TagName]) VALUES (N'C#')
INSERT INTO [dbo].[Tags] ([TagName]) VALUES (N'JAVA')
INSERT INTO [dbo].[Tags] ([TagName]) VALUES (N'JAVA SCRIPT')
INSERT INTO [dbo].[Tags] ([TagName]) VALUES (N'PYTHON')
INSERT INTO [dbo].[Tags] ([TagName]) VALUES (N'TAG 5')

INSERT INTO [dbo].[Users] ([Username], [Password], [FullName], [Email], [Role]) VALUES (N'admin', N'admin', N'admin',N'admin@truc.com', 2)
INSERT INTO [dbo].[Users] ([Username], [Password], [FullName], [Email], [Role]) VALUES (N'user1', N'user1', N'user1',N'user1@truc.com', 1)
INSERT INTO [dbo].[Users] ([Username], [Password], [FullName], [Email], [Role]) VALUES (N'user2', N'user2', N'user2',N'user2@truc.com', 1)
INSERT INTO [dbo].[Users] ([Username], [Password], [FullName], [Email], [Role]) VALUES (N'user3', N'user3', N'user3',N'user3@truc.com', 1)
INSERT INTO [dbo].[Users] ([Username], [Password], [FullName], [Email], [Role]) VALUES (N'user4', N'user4', N'user4',N'user4@truc.com', 1)
INSERT INTO [dbo].[Users] ([Username], [Password], [FullName], [Email], [Role]) VALUES (N'user5', N'user5', N'user5',N'user5@truc.com', 1)

INSERT INTO [dbo].[Posts] ([AuthorId], [Title], [Body], [TimeStamp], [AcceptedAnswer], [ParentId]) VALUES (1, N'Question 1', N'Comment écrire une question', N'2018-07-01 10:11:33', 0, -1)
INSERT INTO [dbo].[Posts] ([AuthorId], [Title], [Body], [TimeStamp], [AcceptedAnswer], [ParentId]) VALUES (1, N'Question 2', N'Comment écrire un commentaire', N'2018-07-02 10:11:33', 0, -1)
INSERT INTO [dbo].[Posts] ([AuthorId], [Title], [Body], [TimeStamp], [AcceptedAnswer], [ParentId]) VALUES (2, N'Question 3', N'Comment écrire une reponse', N'2018-07-03 10:11:33', 0, -1)
INSERT INTO [dbo].[Posts] ([AuthorId], [Title], [Body], [TimeStamp], [AcceptedAnswer], [ParentId]) VALUES (3, N'Question 4', N'Comment écrire une dissert', N'2018-07-04 10:11:33', 0, -1)
INSERT INTO [dbo].[Posts] ([AuthorId], [Title], [Body], [TimeStamp], [AcceptedAnswer], [ParentId]) VALUES (4, N'Question 5', N'Comment écrire une langue', N'2018-07-05 10:11:33', 0, -1)
INSERT INTO [dbo].[Posts] ([AuthorId], [Title], [Body], [TimeStamp], [AcceptedAnswer], [ParentId]) VALUES (5, N'Reponse 1 Q 1', N'Avec le clavier', N'2018-07-06 10:11:33', 0, 1)

INSERT INTO [dbo].[Votes] ([UserId], [PostId], [UpDown]) VALUES (1, 1, 1)
INSERT INTO [dbo].[Votes] ([UserId], [PostId], [UpDown]) VALUES (1, 2, 1)
INSERT INTO [dbo].[Votes] ([UserId], [PostId], [UpDown]) VALUES (1, 3, 1)
INSERT INTO [dbo].[Votes] ([UserId], [PostId], [UpDown]) VALUES (1, 4, 1)
INSERT INTO [dbo].[Votes] ([UserId], [PostId], [UpDown]) VALUES (1, 5, 1)
INSERT INTO [dbo].[Votes] ([UserId], [PostId], [UpDown]) VALUES (2, 1, 1)
INSERT INTO [dbo].[Votes] ([UserId], [PostId], [UpDown]) VALUES (3, 1, 1)
INSERT INTO [dbo].[Votes] ([UserId], [PostId], [UpDown]) VALUES (4, 2, -1)
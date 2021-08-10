using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using PRBD_Framework;

namespace prbd_1920_xyy {
    public enum Role {
        Member = 1,
        Admin = 2
    }

    public class Model : DbContext {
        public Model() : base("prbd_1920_xyy") {
            // la base de données est supprimée et recréée quand le modèle change
            Database.SetInitializer<Model>(new DropCreateDatabaseIfModelChanges<Model>());
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Vote> Votes { get; set; }


        //**********************************************************************
        //**********************************************************************
        //CREATE
        //**********************************************************************
        //**********************************************************************

        //USER
        public User CreateUser (string userName, string password, string fullName, string email)
        {
            var user = Users.Create();
            user.UserName = userName;
            user.Password = Hash(password);
            user.FullName = fullName;
            user.Email = email;
            user.Role = Role.Member;
            Users.Add(user);
            return (user);
        }

        public User CreateUser(string userName, string password, string fullName, string email, Role role)
        {
            var user = Users.Create();
            user.UserName = userName;
            user.Password = Hash(password);
            user.FullName = fullName;
            user.Email = email;
            user.Role = role;
            Users.Add(user);
            return (user);
        }

        //POST
        public Post CreatePost(User author, string title, string body, Post parent, List<Tag> listTag)
        {
            var post = Posts.Create();
            post.Title = title;
            post.Body = body;
            post.AcceptedAnswer = null;
            post.TimeStamp = DateTime.Now;
            // ici on établit les relations dans le sens N-1
            post.Author = author;
            post.Parent = parent;
            // ici on établit les relations dans le sens 1-N
            author.Posts.Add(post);
            if (parent != null)
                parent.Answears.Add(post);
            Posts.Add(post);
            // ici on établit les relations avec les tags
            if (listTag != null)
            {
                foreach (Tag t in listTag)
                {
                    post.Tags.Add(t);
                    t.Posts.Add(post);
                }
            }
            return (post);
        }

        //COMMENT
        public Comment CreateComment(User user, Post post, string body)
        {
            var comment = Comments.Create();
            comment.Body = body;
            comment.TimeStamp = DateTime.Now;
            // ici on établit les relations dans le sens N-1
            comment.Author = user;
            comment.Post = post;
            // ici on établit les relations dans le sens 1-N
            user.Comments.Add(comment);
            post.Comments.Add(comment);
            Comments.Add(comment);
            return (comment);
        }

        //TAG
        public Tag CreateTag (string tagName)
        {
            var tag = Tags.Create();
            tag.TagName = tagName;
            Tags.Add(tag);
            return (tag);
        }

        //VOTE
        public Vote CreateVote(User user, Post post, int nb)
        {
            var vote = Votes.Create();
            vote.UpDown = nb;
            // ici on établit les relations dans le sens N-1
            vote.User = user;
            vote.Post = post;
            // ici on établit les relations dans le sens 1-N
            user.Votes.Add(vote);
            post.Votes.Add(vote);
            Votes.Add(vote);
            return (vote);
        }

        //************************************
        //************************************
        //HASHED PASSWORD
        //************************************
        //************************************

        private string Hash(string pwd)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text  
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(pwd));

            //get hash result after compute it  
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }

        //**********************************************************************
        //**********************************************************************
        //SEED DATA
        //**********************************************************************
        //**********************************************************************
        public void SeedData() {
            if (Users.Count() == 0 && Posts.Count() == 0 && Tags.Count() == 0) {
                Console.Write("Seeding data... ");
                
                ExecManualData();

                Console.WriteLine("ok");
            }
        }

        public void ExecManualData()
        {

            var tag1 = CreateTag("C#");
            var tag2 = CreateTag("JAVA");
            var tag3 = CreateTag("JAVA SCRIPT");
            var tag4 = CreateTag("PYTHON");
            var tag5 = CreateTag("TAG 5");
            SaveChanges();

            var admin = CreateUser("admin", "admin", "admin", "admin@truc.cpm", Role.Admin);
            var user1 = CreateUser("user1", "user1", "user1", "user1@truc.cpm");
            var user2 = CreateUser("user2", "user2", "user2", "user2@truc.cpm");
            var user3 = CreateUser("user3", "user3", "user3", "user3@truc.cpm");
            var user4 = CreateUser("user4", "user4", "user4", "user4@truc.cpm");
            var user5 = CreateUser("user5", "user5", "user5", "user5@truc.cpm");
            SaveChanges();

            var post1 = CreatePost(user1, "Comment écrir un commentaire", "Je voudrais écrire un com, qql sait-il comment faire ?", null, new List<Tag>() { tag2, tag3 });
            var post2 = CreatePost(user1, "Hasher password", "Je voudrais hasher les mot de passe, qql aurait-il une solution à proposer ?", null, new List<Tag>() { tag1 });
            var post3 = CreatePost(user2, "Another Language", "I would like to learn english, is there a simple way to do it ?", null, new List<Tag>() { tag1 });
            var post4 = CreatePost(user3, "Variables", "Je m'y perds entre les int, floats, double, qql pour m'expliquer les différences ?", null, new List<Tag>() { tag1, tag5 });
            var post5 = CreatePost(user4, "Achat maison", "Je recherche une belle maison à vendre, qql vends ?", null, new List<Tag>() { tag2, tag3 });
            var post6 = CreatePost(user5, "Longue Question Ennuyante", "Je suis une tres logue question Je suis une tres logue question Je suis une tres logue question Je suis une tres logue question " +
                " Je suis une tres logue question Je suis une tres logue question Je suis une tres logue question Je suis une tres logue question Je suis une tres logue question ", null, null);
            var post7 = CreatePost(admin, "Administration", "I can't delete an accepted answear, is it normal ?", null, new List<Tag>() { tag2, tag3, tag5 });
            SaveChanges();

            var rep1 = CreatePost(user5, null, "Tu dois cliquer sur commenter gros malin", post1, null);
            var rep2 = CreatePost(user2, null, "Tu dois utiliser des outils de hachage, tel MD5, SHA-256, SHA-12, ...", post2, null);
            var rep3 = CreatePost(user1, null, "Et y a t-il un moyen de le récupérer après ?", post2, null);
            var rep4 = CreatePost(user3, null, "Ce n'est pas fait pour ça, pour te loguer et comfirmer que le mot de passe est le bon, tu dois le faire passer lui aussi dans ta fonction de hachage," + "" +
                " et vérifier que les 2 hachages correspondent", post2, null);
            var rep5 = CreatePost(user4, null, "Maybe u should try the book 'L'anglais pour les nulls' ;-)", post3, null);
            var rep6 = CreatePost(user1, null, "Les int sont des nombres entiers, les floats des nombres décimales, et les doubles, des nombres décimales avec plus de nombres derrière la virgule", post4, null);
            var rep7 = CreatePost(user2, null, "En mémoire, ils sont stocked sur un certains nombre de bits, renseigne toi sur les nombres signés et non-signés :)", post4, null);
            var rep8 = CreatePost(admin, null, "Attention, les nombres ont des valeurs min et max, mais tu peux utiliser le mot clé 'long' pour l'augmentr (max 2x), cherche sur google", post4, null);
            var rep9 = CreatePost(user5, null, "Tu t'es gourré de site mon pote, vas sur Imoweb pour ça", post5, null);
            var rep10 = CreatePost(user1, null, "Non attends, moi j'y un bon plan :)))))", post5, null);
            var rep11 = CreatePost(user1, null, "Je suis pas admin, mais j'imagine que ça n'aurait aucun sens", post7, null);
            var rep12 = CreatePost(user3, null, "Je suis du même avis que User2, supprime le post si tu ne veux plus le voir", post7, null);

            post1.AcceptedAnswer = rep1;
            post7.AcceptedAnswer = rep11;
            SaveChanges();

            var vote1 = CreateVote(user1, post3, -1);
            var vote2 = CreateVote(user1, post4, 1);
            var vote3 = CreateVote(user1, post5, -1);
            var vote4 = CreateVote(user1, post6, 1);
            var vote5 = CreateVote(user1, post7, 1);
            var vote6 = CreateVote(user1, rep5, 1);
            var vote7 = CreateVote(user1, rep2, 1);
            var vote8 = CreateVote(user1, rep4, 1);
            var vote9 = CreateVote(user1, rep12, 1);

            var vote10 = CreateVote(user2, post1, 1);
            var vote11 = CreateVote(user2, post2, 1);
            var vote12 = CreateVote(user2, post4, -1);
            var vote13 = CreateVote(user2, rep5, 1);

            var vote14 = CreateVote(user2, post1, 1);
            var vote15 = CreateVote(user2, post2, 1);
            var vote16 = CreateVote(user2, post3, -1);
            var vote17 = CreateVote(user2, post4, 1);
            var vote18 = CreateVote(user2, post5, -1);
            var vote19 = CreateVote(admin, rep11, 1);
            var vote20 = CreateVote(admin, rep12, 1);
            SaveChanges();

            var comment1 = CreateComment(user1, post1, "Et pq pas en majuscule");
            var comment2 = CreateComment(user1, post1, "Et en italique");
            var comment3 = CreateComment(user1, post6, "Je suis le premier comment");
            var comment4 = CreateComment(user3, post6, "Je suis le deuxieme comment");
            var comment5 = CreateComment(admin, post6, "Long commentaire, oui oui, oui oui, très très très long, enfait non pas si long que ça");
            var comment6 = CreateComment(admin, post6, "Et moi j suis moins long");
            var comment7 = CreateComment(admin, rep11, "C'est malin ça");
            var comment8 = CreateComment(admin, rep11, "Merci beaucoup");
            var comment9 = CreateComment(admin, rep12, "Je n'y avais pas pensé");
            var comment10 = CreateComment(admin, rep12, "Vous devrize être admin");
            SaveChanges();
        }
    }
}
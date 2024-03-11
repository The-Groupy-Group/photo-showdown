using AutoMapper;
using PhotoShowdownBackend.Exceptions;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.CustomSentences;
using PhotoShowdownBackend.Repositories.MatchConnections;
using PhotoShowdownBackend.Services.MatchConnections;
using System.Collections.Concurrent;

namespace PhotoShowdownBackend.Services.CustomSentences
{
    public class SentencesService : ISentencesService
    {
        private readonly ICustomSentencesRepository _customSentencesRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<SentencesService> _logger;
        private static readonly ConcurrentDictionary<int, ConcurrentBag<string>> _defaultSentencesRepo = new();
        private static readonly List<string> _defaultSentences = new()
        {
            // Maor's sentences
            "The best sentence to start talking to a girl ___.",
            "I went to the grocery store and asked for my  favorite food which is___.",
            "On Christmas my aunt brought me the gift I wanted the most.",
            "I hate washing dishes, it always ends with___.",
            "At my birthday party I invited all my friends except___, whom I don't like.",
            "When she asks me to___, I always tell  her___.",
            "The best way to be in two places at the same time___.",
            "With great power comes great responsibility.",
            "If I had one wish I would ask___.",
            "The one thing i will never do___.",
            "Most of the times i help a friend, i expect for return___.",
            "My favorite game is___, I always win.",
            "My favorite sentence.",

            // Gepeto's sentences
            "When you accidentally send a text meant for your crush to your boss.",
            "Trying to adult like... picture of a toddler in a suit",
            "When you realize your dance moves are better in your imagination.",
            "That face you make when someone says something so absurd, you question reality." ,
            "Attempting to parallel park: Expectation vs. Reality."                           ,
            "When your GPS says 'Turn left' but there's a lake on the left."                  ,
            "Trying to impress your crush like... picture of a cat wearing sunglasses"        ,
            "Monday mornings summed up in one picture."                                       ,
            "When autocorrect changes 'wine' to 'whine' – accurate."                          ,
            "That feeling when you successfully assemble IKEA furniture on the first try."    ,
            "Reacting to a bad joke like... insert funny reaction face"                       ,
            "When your WiFi goes down and you have to get to know your surroundings – also known as 'outside.'"         ,
            "Trying to look casual after tripping in public."   ,
            "Attempting a DIY project: Nailed it or failed it?" ,
            "When you realize adulthood is just a never-ending cycle of doing dishes."                                  ,
            "That awkward moment when you wave at someone, but it turns out they were waving at the person behind you." ,
            "When you put something in a 'safe place' and never find it again."                               ,
            "Trying to sneak out of a Zoom meeting unnoticed."                                                ,
            "When you hear someone talking about a TV show you haven't watched: insert clueless face"         ,
            "Attempting to take a selfie with a pet: Mission impossible."                                     ,
            "When you finally understand a math problem after staring at it for hours."                       ,
            "Trying to look interested during a long meeting: picture of someone with exaggerated enthusiasm" ,
            "That moment when you see your food coming at a restaurant.",
            "When you accidentally like someone's post from five years ago."    ,
            "Attempting to look cool while wearing 3D glasses."                 ,
            "When your phone autocorrects 'ducking' for the millionth time."    ,
            "That face you make when you see your crush in public unexpectedly.",
            "Trying to hold in a laugh at an inappropriate moment."             ,
            "When you realize you've been singing the wrong lyrics for years."  ,
            "Attempting to take a group photo: picture of everyone looking in different directions" ,
            "That feeling when you finally get a good night's sleep."                 ,
            "When you open the fridge expecting something new, but it's still empty." ,
            "Trying to understand modern art: picture of confused face"               ,
            "When you accidentally send a screenshot to the person you screenshotted.",
            "Attempting to dance like nobody's watching: insert hilarious dance move" ,
            "That moment when you see a spider in your room."                         ,
            "When you realize your outfit looked better in your head."                ,
            "Trying to impress your parents with your cooking skills: picture of burnt food"  ,
            "When you accidentally send a voice message of you singing in the shower."        ,
            "That awkward silence after telling a joke and no one laughs."                    ,
            "Attempting to take a cute couple selfie: picture of failed attempts"             ,
            "When you hear your own voice in a recording and question if that's really you."  ,
            "Trying to keep a straight face during a serious conversation."                   ,
            "When you see your ex in public and try to act nonchalant.",
            "That face you make when you accidentally send a text to the wrong person."               ,
            "When you realize you've been singing a children's song all day."                         ,
            "Trying to take the perfect food picture for Instagram: picture of elaborate setup"       ,
            "When you hear someone talking about a movie you haven't seen: insert confused expression",
            "Attempting to understand a new technology: picture of someone staring at a smartphone"   ,
            "That feeling when you successfully navigate a conversation without revealing you haven't read the book.",
            "When you try to impress your crush with a witty comeback: picture of dramatic pose"       ,
            "Trying to act normal when your stomach growls loudly in a quiet room."                    ,
            "When you accidentally like your own post."                                                ,
            "Attempting to take a serious photo with a pet: picture of pet doing something silly"      ,
            "That moment when you realize you left your phone at home."                                ,
            "When you try to sneak a snack during a meeting: picture of stealthy snacking"             ,
            "Trying to impress your crush with your cooking skills: picture of smoke alarm going off"  ,
            "When you accidentally send a message meant for your friend to your mom."                  ,
            "Attempting to look sophisticated while eating spaghetti: picture of spaghetti on face"    ,
            "That face you make when someone tries to tickle you."            ,
            "When you realize you've been singing the wrong national anthem." ,
            "Trying to take a group photo with friends: picture of everyone making silly faces"  ,
            "When you accidentally reply 'you too' to the waiter saying 'enjoy your meal.'"      ,
            "Attempting to parallel park: Expectation vs. Reality, part 3."                      ,
            "That feeling when you remember something embarrassing from five years ago."         ,
            "Trying to impress your crush with your sports skills: picture of failed attempt"    ,
            "Attempting to take the perfect selfie: picture of multiple attempts"                ,
            "That moment when you realize you've been pronouncing a word wrong your entire life.",
            "When you hear someone talking about a TV show you haven't watched, part 2: insert even more clueless face",
            "Attempting to take a cute couple selfie: picture of more failed attempts",
            "Trying to understand modern art: picture of even more confused face",
            "Trying to impress your crush with your cooking skills: picture of kitchen disaster"  ,
            "When autocorrect changes 'wine' to 'whine' – still accurate.",
            "Trying to take a serious photo with a pet: picture of pet refusing to cooperate",
            "Attempting to take a cute couple selfie: picture of more and more failed attempts" ,
            "Trying to take a group photo with friends: picture of everyone making even sillier faces",

            // Gemini's sentences
            "You accidentally walk in on your parents making out.",
            "You try to parallel park and end up creating a modern art installation.",
            "Your phone autocorrects 'I love you' to 'I'm a slug.'",
            "You realize you've been telling the same embarrassing story to everyone for years.",
            "You trip and fall flat on your face in front of your crush.",
            "You accidentally wear mismatched socks to an important meeting.",
            "Your internet cuts out right before you win an online game.",
            "You try to sing along to a song and completely butcher the lyrics.",
            "You spend all day cleaning your house, only for your family to completely mess it up again.",
            "You offer to help someone carry groceries and accidentally drop the entire bag.",
            "Observational Humor:",
            "Why do we call it an 'alphabet' but not a 'numberbet'?",
            "If you ever feel useless, remember that keyboards have a scroll lock key.",
            "I don't trust stairs. They're always up to something.",
            "Money can't buy happiness, but it can buy ice cream, and that's kind of the same thing.",
            "I'm not lazy, I'm just highly motivated to do nothing.",
            "Adulting is basically just yelling at things until they work.",
            "I wish I could steal the confidence of people who leave grocery stores without checking their receipts.",
            "My bank account is like a black hole. Money goes in, but nothing ever comes out.",
            "Sleep is like a drug. I do it every night and still need more.",
            "Why does it feel like Monday lasts all week, but the weekend flies by in 5 minutes?",
            "Self-Deprecating Humor:",
            "I'm not good at multitasking. I can barely even function at singletasking.",
            "My love life is like a Netflix show. Everyone else is enjoying the third season, and I'm still stuck on the pilot.",
            "I'm so bad at taking compliments. I just stand there awkwardly and try to disappear.",
            "My brain is 80% sarcasm, 15% pop culture references, and 5% actual knowledge.",
            "I'm on a seafood diet. I see food, and I eat it.",
            "My spirit animal is a sloth. We both enjoy napping and doing absolutely nothing.",
            "I'm an adult, but sometimes I still need my mom to cut my sandwich crusts off.",
            "I'm not sure what's wrong with my life, but the universe seems to be sending me a lot of signs that I should stay in bed.",
            "I'm not anti-social, I'm just selectively social. I only like hanging out with people who are better than me. (Just kidding, nobody is better than me.)",
            "I'm allergic to responsibility. It gives me hives.",
            "Pop Culture References:",
            "\"I'm in danger!\" - Every character in a horror movie ever.",
            "\"Pivot!\" - Ross Geller from \"Friends\"",
            "\"That's what she said!\" - Michael Scott from \"The Office\"",
            "\"Do you even lift?\" - Gym bros everywhere",
            "\"Wakanda forever!\" - Black Panther fans",
            "\"I used to be with it, then they changed what 'it' was. Now what I'm with isn't 'it' anymore.\" - Grandpa from \"The Simpsons\"",
            "\"It's Wednesday, my dudes!\" - Internet meme",
            "\"They're taking the hobbits to Isengard!\" - Merry and Pippin from \"Lord of the Rings\"",
            "\"I am Groot!\" - Groot from \"Guardians of the Galaxy\"",
            "\"May the odds be ever in your favor.\" - Effie Trinket from \"The Hunger Games\"",
            "Rude/Insulting Humor (use with caution):",
            "My ex texted me \"u up?\" last night. I replied \"u lost?\"",
            "I'm on a seafood diet. I see food, and I eat it. (You might have already used this one, but it fits here too!)",
            "I don't know what's more annoying, people who brag or people who fish for compliments.",
            "I'm not saying I hate you, but I would unplug your life support to charge my phone."

        };

        public SentencesService(ICustomSentencesRepository customSentencesRepo, IMapper mapper, ILogger<SentencesService> logger)
        {
            _customSentencesRepo = customSentencesRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<string?> FetchSentence(int matchId)
        {
            var customSentence = await _customSentencesRepo.GetAsync(cs => cs.MatchId == matchId);

            if (customSentence == null)
            {
                ConcurrentBag<string> SentencesListForMatch = _defaultSentencesRepo.GetOrAdd(matchId, matchId => InitDefaultSentencesForMatch());
                SentencesListForMatch.TryTake(out string? defaultSentence);
                return defaultSentence;
            }
            else
            {
                await _customSentencesRepo.DeleteAsync(customSentence);
                return customSentence.Sentence;
            }
        }

        public async Task SetCustomSentences(List<string> sentenes, int matchId)
        {
            await _customSentencesRepo.CreateRangeAsync(
                sentenes
                .Select(s => new CustomSentence
                {
                    Sentence = s,
                    MatchId = matchId
                })
                .ToArray());
        }

        private static ConcurrentBag<string> InitDefaultSentencesForMatch()
        {
            Random rnd = new();
            ConcurrentBag<string> sentences = new();
            _defaultSentences.OrderBy(x => rnd.Next()).ToList().ForEach(sentences.Add);
            return sentences;
        }
    }
}

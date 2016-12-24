Imports System.Security.Cryptography
Imports System.Text
Imports Helper.RandomizeHelper.RandomizerType

Namespace RandomizeHelper
    Public NotInheritable Class Randomizer

#Region " Fields "
        Private Shared m_maxLength% = 1
        Private Shared m_generatedNames As New List(Of String)()
        Private Shared m_alphabeticChars As Char()() = {"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray()}
        Private Shared m_japenese$ = "れづれなるまゝに日暮らし硯にむかひて心にうりゆくよな事を、こはかとなく書きつくればあやうこそものぐるほけれ。"
        Private Shared m_greek$ = "αβγδεζηθικλµνξοπρστυϕχψω"
        Private Shared m_Symbols$ = "☹☺☻☼☽☾☿♀♁♂♔♕♖♗♘♙♚♛♜♝♞♟♠♡♢♣♤♥♦♧♩♪♫♬♭♮♯♻♼♿⚐⚑⚒⚓⚔⚕⚖⚠⚢⚣⚤⚥⚦⚧⚨⚩⛄⛅⚽✔✓✕✖✗✘✝✞✟❗❓❤☸"
        Private Shared m_rdn As Random = New Random()
        Private Shared m_rdnBool = New Random(DateTime.Now.Millisecond)
        Private Shared m_reservedWords As String() = New String() {"addhandler", "addressof", "alias", "and", "andalso", "ansi", "append", "as", "assembly", "auto", "binary", "boolean", "byref", "byte", "byval", "call", "case", "catch", "cbool", "cbyte", "cchar", "cdate", "cdec", "cdbl", "char", "cint", "class", "clng", "cobj", "compare", "const", "continue", "cbyte", "cshort", "csng", "cstr", "ctype", "cuint", "culng", "cushort", "date", "decimal", "declare", "default", "delegate", "dim", "directcast", "do", "double", "each", "else", "elseif", "end", "endif", "enum", "erase", "error", "event", "explicit", "exit", "false", "finally", "for", "friend", "function", "get", "gettype", "global", "gosub", "goto", "handles", "if", "implements", "imports", "in", "inherits", "input", "integer", "interface", "internal", "is", "isnot", "let", "lib", "like", "lock", "long", "loop", "me", "mid", "mod", "module", "notinheritable", "mustinherit", "mustoverride", "my", "mybase", "myclass", "namespace", "narrowing", "new", "next", "not", "nothing", "notinheritable", "notoverridable", "object", "of", "off", "on", "operator", "option", "optional", "or", "orelse", "output", "overloads", "overridable", "overrides", "paramarray", "preserve", "partial", "private", "property", "protected", "public", "raiseevent", "random", "read", "readonly", "redim", "rem", "removehandler", "resume", "return", "sbyte", "seek", "select", "set", "shadows", "shared", "short", "single", "static", "step", "stop", "strict", "string", "structure", "sub", "synclock", "then", "throw", "to", "true", "try", "trycast", "typeof", "variant", "wend", "uinteger", "ulong", "until", "ushort", "using", "void", "when", "while", "widening", "with", "withevents", "writeonly", "xor", "region"}
        Private Shared m_flowing As String = ".̃̏̂͑̽̿̄ͯ҉̬̬̭͉͙.ͮͦ͌̑̄ͤͥͫ̕͏͉̖̪̪̤͈̝̠͓̥̹̥̺͉ͅ.̶̸̘̘̙̭̻͖̪͍̖̰͍͇̟̯̖̃͒ͧ̈́ͧ͛͑ͣ̂̌͑̽̎̔̚͢.͙̙͓͔̻̺̪͙̼̺͙̠̭̣̯̫͔̭ͥͦͫ͜͟͞.̩͈̤̼̬̬̼̘͎̻̼̠̼͓̝̯̰̌̆̋̃͗̃̅͂̂̈́̔͒̑̕͜.̧̭̫̭̮͙̣̺̳̦̝̦͍͚̟̯̟̽̂ͨ͑̈̀͟͠.̡̣̖͚̮͓͇͔͈̱̯̞͓̙̞͕͚́ͩ̾ͯ̍̏̿̆ͫ̆͛̑́͝ͅ.̛̊ͨ̆͂͌ͭ͏̸͓͕͕͓̗͙͇̤͍̦͕̥̘͇.̡͔͇͍̦͚̲͔̯̪̙̘͓͚̬̲͔̼͕̽̃̉ͫ̓̑ͫ̉ͫ̒̊͜.͎̹̫͕ͯ̐͌̐͒͛̐̎̏ͨͮ͂̒̀̚͠.̶̷̨̝̣͖͇̲̯͇̰͈̙͉͙͚͉̄͑͗̏̒ͪ̏ͮ͌͗ͬͪͥͭ̊͋͞.̜̟͕̺̣͕̥͚͔͓̠̞̳̭̠ͪ̽ͭ͒ͮ͘͝.̛̝̦͎͚̈̉̈́̈͛ͯ͑ͫ̊ͮͬ̆ͣ͂ͥ.̸͇̮͙͈͇̱͈͕̜̬̻̮ͫ͊ͭ̏͑̔͐̑ͬ̾̂ͩ͆ͫ̀.ͤ͂ͩ̀͑̒͏̷́͏͓̮̙͈̮̳̲̭̺̟̱̞͍̜̥͜.̶̷̣̮͍͇͈̝̞͓̦͐ͤͦͤͦͭ̆͒̓̀ͫ̅̐̚͡.̵̢͇̯͕͕̤̥̘͍͂̆̊ͮ̆̋̿ͧ͊ͩ̑͜.̸̧̗̜̼͖̲̟̹̞̈ͭ̊̔ͪ̐ͤ͆̇̔ͫͮ̀́.̏̅̃̓ͭ͏͇̲͎̹̖͙͎̯̥͡.̢̰͕̭̲͖͇͒́̾͋ͬ̅̈͑ͥͅͅ.͊̒̃͒́͜͏̢͉̲̹̼̥̥͖̘̼̹͈͉.̸̷̶̜̞͖̪̻̦͕͕̼̮̳͙̯̹̩̗̓͌̑ͭ̏͂̾͂̒ͭ̍̀̚.̵̡̘͕͚̳͐ͦͥ̉͘͢ͅ.̶̸̨̨͍̳̣̱͓̫̫̱̖̣͔̅ͧ̂́ͯ̓͋͋͂̾͑̈́̇̑̎̑ͭ̍͜.̴̵̘̩͍͖̻̦̣͕̗̖͔̘͓͗̈͛͂́̾ͫ͛̄͆ͤ̑͘͘.̡̛̮͇̫̮͔̲͕̫̹̘̞̱̾̈ͬ̆ͦ̈́͂̀̌̈́̆͋͆͋́.̶̴̨̩̻̮̹͔̞̻͖̭̻̲̉͆̓ͨͥ̈́̈́ͤ̅͑̆̑̔̔̍̀͘͝.̃̏̂͑̽̿̄ͯ҉̬̬̭͉͙.ͮͦ͌̑̄ͤͥͫ̕͏͉̖̪̪̤͈̝̠͓̥̹̥̺͉ͅ.̶̸̘̘̙̭̻͖̪͍̖̰͍͇̟̯̖̃͒ͧ̈́ͧ͛͑ͣ̂̌͑̽̎̔̚͢.͙̙͓͔̻̺̪͙̼̺͙̠̭̣̯̫͔̭ͥͦͫ͜͟͞.̩͈̤̼̬̬̼̘͎̻̼̠̼͓̝̯̰̌̆̋̃͗̃̅͂̂̈́̔͒̑̕͜.̧̭̫̭̮͙̣̺̳̦̝̦͍͚̟̯̟̽̂ͨ͑̈̀͟͠.̡̣̖͚̮͓͇͔͈̱̯̞͓̙̞͕͚́ͩ̾ͯ̍̏̿̆ͫ̆͛̑́͝ͅ.̛̊ͨ̆͂͌ͭ͏̸͓͕͕͓̗͙͇̤͍̦͕̥̘͇.̡͔͇͍̦͚̲͔̯̪̙̘͓͚̬̲͔̼͕̽̃̉ͫ̓̑ͫ̉ͫ̒̊͜.͎̹̫͕ͯ̐͌̐͒͛̐̎̏ͨͮ͂̒̀̚͠.̶̷̨̝̣͖͇̲̯͇̰͈̙͉͙͚͉̄͑͗̏̒ͪ̏ͮ͌͗ͬͪͥͭ̊͋͞.̜̟͕̺̣͕̥͚͔͓̠̞̳̭̠ͪ̽ͭ͒ͮ͘͝.̛̝̦͎͚̈̉̈́̈͛ͯ͑ͫ̊ͮͬ̆ͣ͂ͥ.̸͇̮͙͈͇̱͈͕̜̬̻̮ͫ͊ͭ̏͑̔͐̑ͬ̾̂ͩ͆ͫ̀.ͤ͂ͩ̀͑̒͏̷́͏͓̮̙͈̮̳̲̭̺̟̱̞͍̜̥͜.̶̷̣̮͍͇͈̝̞͓̦͐ͤͦͤͦͭ̆͒̓̀ͫ̅̐̚͡.̵̢͇̯͕͕̤̥̘͍͂̆̊ͮ̆̋̿ͧ͊ͩ̑͜.̸̧̗̜̼͖̲̟̹̞̈ͭ̊̔ͪ̐ͤ͆̇̔ͫͮ̀́.̏̅̃̓ͭ͏͇̲͎̹̖͙͎̯̥͡.̢̰͕̭̲͖͇͒́̾͋ͬ̅̈͑ͥͅͅ.͊̒̃͒́͜͏̢͉̲̹̼̥̥͖̘̼̹͈͉.̸̷̶̜̞͖̪̻̦͕͕̼̮̳͙̯̹̩̗̓͌̑ͭ̏͂̾͂̒ͭ̍̀̚.̵̡̘͕͚̳͐ͦͥ̉͘͢ͅ.̶̸̨̨͍̳̣̱͓̫̫̱̖̣͔̅ͧ̂́ͯ̓͋͋͂̾͑̈́̇̑̎̑ͭ̍͜.̴̵̘̩͍͖̻̦̣͕̗̖͔̘͓͗̈͛͂́̾ͫ͛̄͆ͤ̑͘͘.̡̛̮͇̫̮͔̲͕̫̹̘̞̱̾̈ͬ̆ͦ̈́͂̀̌̈́̆͋͆͋́.̶̴̨̩̻̮̹͔̞̻͖̭̻̲̉͆̓ͨͥ̈́̈́ͤ̅͑̆̑̔̔̍̀͘͝ḩ̷̸͎̞̬͚͙́͒̃̿̑ส็็็็็็็็็็็็็็็็็็็i͇̠̱̽͛ͣͯͭ̐͐ͩͪ̀͒̿̍̆̌ͣ̕͞ţ̈́̄ͦ͑͐ͤ̇ͯ̚͜͢͏̺͎̰̯̰̳̣̺͉͉̻̯̱͉̱̳̠̫l̢̮̝̰̖̲̯͉̱͉̤̗̯͇ͫ͋͑͋͊́͑͠e̛̼͉̝̯̼͚͇̜̹̬̼͚̥̝̟̩̮̎̾ͧ͟͝ͅr̷͎̣͙͇̦̱̺͚̬͍͎̗̺͍͈͍̔̃̆ͬ̃͌ͦ͗ͧ̓͋̓͟͟͡ͅ ̵̩̼͙̣̦͕̃ͨͧ̂ͭ͂̀͜ḣ͚͖͉͓̫̲̦͓́̆̈ͯ͒͂ͫ͛ͣ̓ͫ̄́́̕͜͜͜ą̴̢̺̼͎̩͓̱͍̯͓̻̖͓̯̿ͩͩͦ̕ͅt͂ͫ̔͋͆̀ͩͨ͂̎̓ͧ̿̈́̓̏̃ͯ̈͘͘҉͚̬̝͙̟̗̰̹̱̗ ̵̠̤̼̬̩͔̲̖̎̍̈̌̾̎̋̂̓ͬ̒ͫ̽ͭ́̕͠n̛̲͙̻̤̮̥̠͇͇͖͎̘̠̲ͥͣ̋͛ͨ̀i̧͓͖͈̭͔͉̼ͪͫ̓͂̔̿͠ͅč̨̆̉͂̑͞͏̻̠̖̼̹̻̹̯͇͙̰̪̯hͭ͌ͦ̉̊͐͂҉̸͇̹̹̬̖͕̱͕͕̠̗̀͘͝tͤͫͫͧ̈́͂͛ͭ̉ͧ͛ͫ̚҉̴͚̯̭̲̫̦͖̮̭͖̗͎̳̟̀͘s̸ͧ͗̌͊ͨ̐̅̇͟҉͈̤̘͉̤̯̝͈͚ ͤ͆͆ͨ̓҉̤̣̩̠̩̯̩̱͕̹͜͝f̡̞͔̮͖̩͔̀̆̅̓̈ͪͥ͋͊̉ͪ̇̉̃̔͋̃̈́͟͠͝ä̵̸̺̖͓͖̳̬̲̲͎͎͔̈͋͑͋ͦͅͅl̻̟̲̞̘͚̤͎͉̯̫̹̜̥̳͈̙ͧ͋̇ͫ͋̎ͯ̋͂ͮ̈͂̾ͯ̎̊̾ͯ͘͢ͅs̴̡͍̖̖͕̱̫̤̣͛ͩͤ͆̅ͣ͐̿ͣ͐̔ͨ̄ͫͩ̄̍͘̕͜c̵̷̨͇̰̰̼̝̝̼̤͎̯̺̰͕̤̤͇ͧͦ̄̇̓ͩ̎͂̊ͯ͋͋̋̀ͬͧ͗̔̚͝͡ͅͅh͓͙̩̭̬̠̜͇̗̮̐ͥͧͭ̆͆̔ͬ́̄͊ͮ͡͝͠ ̩̹̥̯̲͉͔̟͕͎̪͔̱̬̌ͭ͗̔̏̊̚͡͞g̴̴̫͖̥̲̦͉̩̲̪̹̙̘̩̣̯̜̱͌ͩ͑̆̿̏̽ͤ͂ͩ́͢͠e̤̳͈̹͉̹̪̥̜̲͙͕͍̟̱̱̳͗̽ͤ̈́̽̽̊͢͡ḿ͈̮͓͇̞̯͍̦͖̟͔̫͈̏̑̋̂̒ͬ͌̌̓̄͢͞ą̶̴͈̳̥͙͚͓͉̟̬̤͋͒ͫ̓̿́̒͐͒͘͡c̡̞̪̞̣̦̖̙̬̜̜̋̿͐̇̓̋̃͆̚h̨̛̝͓͚̱͍͕̝̬̯̩̓̽̉͌͊̇͐̒̈͋͋̌ͩ̋ͭt̶̄́͑̐҉҉̟̭̟͓̜̩͖̲͔̀ ̷̧̡̦̞̝̯̥͍̻͚̠̞̣̯͎̇̓̇̐̓́͝h̡̺̳͇̤̬̻̮̭͇̿̑̔̽ͮ̉̏̃͂̄̌̍͒̑̇ͪ̑͡͞ĭ̵͔͙̞̻̰̻̬̖͇̩͔̪̮̩̘̔ͪͥ̈́͋͞t̸̝̪̹̲̤̜̓͌ͤ̍ͫͧ͋͋ͣ͆̈́ͩͯ̒ͮ̊ͧ̕͢͜ͅl̈́ͦ̈̌͆ͧ͏̷̛̮̼̬͓̞̻̣̼͙ͅe̗͚̺̰͎͖̥̙̻͕̮͕̱͇̓̑ͫ͋ͭͯ̍ͨ̌͗̊̔̒̈́̽̕͜͝ͅṛ̶̷̟̹̥̹̬̖͖͇̬̭̲̬̠̮ͣͧ̓̓̈́̅͢ ̨̛̘̲͉̮̲̹̳͔̗̣̣̗̱̱̘͚ͦ͋̀̍̃̑ͣ̍͒̇ͭ̆ͧ͒͒̋̕h̷͔̠͈̙̝̻̺͔͕̤ͯ̃̎̋͑ͫ̎̾̃̿̀̄́̀͞a̍̏͛̔͆͗͒̆̐́̈ͥ̃͌͝҉̧҉̷̪̳̻͓̘̜̘͔̘̞̱̫͈̹ͅť̷̢̫̰̦̫̯̮̟͇̍͊̒̅̀̐͊ͯ́̒̅ͤͅ ̵̮̤̳̺̤̼̝̉̑̎ͧ̏̀̽̽́n̴̴̯̹̮̣͉̹̝̑͑̐̿ͮ̈̆̔́̏ͥ͋ͨ̒͘͘͟i̲̬͓̯̋͑̂̋͋͡c͑̒̈́ͫ̓̎͆̃̃ͬͯ͜͏͓̲̝̺̥̘͍͚̕ḩ̜͖̤̼͇̳̳̭̻͖̳̙͑͌̑̓͒ͦ͂ͮ̅̅͌̈ͭͭ̄ͅṫ̴͎̺̭̺̞̺̮̼̣͔͎͔̗̱̭̄̆̓͋̔͝s̛͓͇̮̹̙̫̮̦̪̜̋ͪ̊̊ͮ̎͌̂̂̿̽̔̉̓̍̔͆̕ ̛̽͌͋ͩ̃̈̍ͨ̅ͦ̀̏̍̓̑̍̊ͧ͘͏̪͔͇ͅf̢̰̲̺͙̝̣͕̭̝̙͍ͧ̌ͤͮ͛͋ͭ́̓̽̔̈́̂ͤ̉̆ͩ̚͘ͅa̧̡ͪ̊̊ͩ̈͠͏҉̠̬̝̻͙̰̖̻̼̖̘̠̺̝l̸̷͊͒ͨ̄̓̂҉̥̩̮̳̯̠̻͎̹͈̟̠̫̮̫̠̖̥͙s̶͒̓̇͑͗̍̿̐ͮ҉͇͖͓̣͉̗̰̯͎̖͎̱c̶͇̭̣͍̞̝͓͇̫̯̜̫̞͉̑ͤͧ̎̒̈ͯͣͥ̍ͪ̌̎̒́h̵̢̼̮̖͎̭̭͇͚̮͙͙͇̗̤̝̺͚̐̏͋ͬ̋̿̎ͭ̂̾̂̓ͪ̏̋̀ ̵̨̛̱̖̫͇̱͈̞̭̱́̔ͪ̋̎̎ͭͨ̈̿ͤ̎̿͟ͅg̉͂͛̍̓ͫ̇̿̎͐̓̏͏͏̴̤̗̙͖͙̱̺͎̖̩͝ę̵̞͖̭̳͔̻͉̯̻̯̣̈́́̈̊̽̾͗ͣ̃͊ͬ̔ṃ̶̳̫̲̩̺͍̝̰̻̱̖̦̪̘̠̏ͫ̎ͤ̓ͣ̾͊̑͒͗̋͂̉̈́͜͞a̰̯͔̗̠͖̣̬̖͐ͬ̏͐ͬ͊̂̍̇ͣͩ́͠͞c̨̡̬̥̟̩̠̬̟̪͖̙̮̺̩͍̮̝ͥ̌̉ͣ͊̈͌̓̈̉̈͐̿̈ͬ̀h̛̬͚̝͉̮̝͉̥̺̩̼̞̙̖͎̮̳̒͂̋̋ͬ̓͋̂̊̚̕t͒ͮ̿ͤͬ̎ͭ͌̅̂̾̐̉ͦ͌ͧͯ͋̈͏̮̮̰̻̕͜͞ ̸͎̥̞̟͙͚͙̥ͬ̈́̋̔̿ͣͨͧͫ͒̿ͬͫͧ̊͢͜͠ḫ̝̘̤̰͐͆ͤ̑̒͛ͩ́͞iͤ̓͑̊̏ͥ̀͘͞҉̨̗̬͉̜̘̜t̑ͨ͋̾ͦ̋̊ͤ̔̒̑̿̓́͏̵̛̱͚̖͓̲̕l̶͍̳̳͎͓̀ͭ̎̉̌̓̊̌̍̍̀̕e̵̡͇͈͉̥̼̼̺͎͉̦ͮ̾̒̃ͫ̃͒̃̓́̅͆ͬ͗ͨ̿ͥ̂̚͝͡ŗ̡̊͋̾ͩ̽͆̋̈́͐͊̂ͮͨ̉̏ͨ͐͐̆̕͞͏͍͍̰̻̖̥ ͨ́̿̃̇ͯ̾̕͢҉̩͇̟̪̥̬͍̲͈͔͔͍̼̭͇̣h̷̻̝̫̪͚̦͙͉͎̥̦̳͉̖̃ͥ̅͗͢ḁ̛̪̗̮̦̳̪̭̞̗̠̟̈́̌͐̈́ͦ̄̉̎ͬ͆̒̉̕͟t̶̛͔͍͔͎̫̞̖͓̰̒̇̆ͯ̀ͥ̈́̏̓̀ͮͥ̍̀ ̨ͥ̄̓ͩ̿̃̿̊̈̔͒͗͊ͭ̽ͥͥ͐҉̶̸̲̻̱̩̖̪̹͈̙̩͎̲̘̙n"
        Public Shared invisibleChars As Integer() = New Integer(3) {8203, 8204, 8206, 8207}
#End Region

#Region " Methods "

        Public Shared Function GenerateInvisible() As Integer
            Dim b = invisibleChars
            Return m_rdn.Next(b(0), b(3))
        End Function

        Public Shared Function GenerateBoolean() As Boolean
            Dim b = New Integer(9) {5, 8, 1, 7, 3, 2, 9, 0, 4, 6}
            Return (Odd(b(m_rdnBool.Next(10))))
        End Function

        Public Shared Function GenerateNumber() As Integer
            Dim b = New Integer(1) {0, 1}
            Return m_rdnBool.Next(2)
        End Function

        Private Shared Function Odd(value%) As Boolean
            Return (value And 1) = 1
        End Function

        ''' <summary>
        ''' Creates a pseudo-random password containing the number of character classes
        ''' </summary>
        Private Shared Function GenerateKey(length%) As String
            Dim csp As New System.Security.Cryptography.RNGCryptoServiceProvider()

            Dim allchars = m_alphabeticChars.Take(1).SelectMany(Function(c) c).ToArray()
            Dim bytes = New Byte(allchars.Length - 1) {}
            csp.GetBytes(bytes)

            For i% = 0 To allchars.Length - 1
                Dim tmp = allchars(i)
                allchars(i) = allchars(bytes(i) Mod allchars.Length)
                allchars(bytes(i) Mod allchars.Length) = tmp
            Next
            Array.Resize(bytes, length)
            Dim result = New Char(length - 1) {}
            While True
                csp.GetBytes(bytes)
                For i% = 0 To length - 1
                    result(i) = allchars(bytes(i) Mod allchars.Length)
                Next
                If Char.IsWhiteSpace(result(0)) OrElse Char.IsWhiteSpace(result((length - 1) Mod length)) Then
                    Continue While
                End If

                Dim testResult As New String(result)
                If 0 <> m_alphabeticChars.Take(1).Count(Function(c) testResult.IndexOfAny(c) < 0) Then
                    Continue While
                End If

                Return testResult
            End While
            Return m_alphabeticChars.Take(1).ToString
        End Function

        Private Shared Function Alphabetic() As String
            Dim str$ = GenerateKey(m_maxLength)
            If m_generatedNames.Contains(str.ToLower) OrElse m_reservedWords.Contains(str.ToLower) Then
                m_maxLength += 1
                Return Alphabetic()
            End If
            m_generatedNames.Add(str.ToLower)
            Return str
        End Function

        Public Shared Function GenerateNewAlphabetic() As String
            Return Alphabetic()
        End Function

        Private Shared Function japanese() As String
            Dim str$ = New String(Enumerable.Repeat(m_japenese, m_rdn.Next(1, m_maxLength)).Select(Function(s) s(m_rdn.Next(s.Length))).ToArray())
            If m_generatedNames.Contains(str) Then
                m_maxLength += 1
                Return japanese()
            End If
            m_generatedNames.Add(str)
            Return str
        End Function

        Private Shared Function Dots() As String
            Dim builder As New StringBuilder()
            Dim randomize As New Random(Guid.NewGuid().GetHashCode())

            For i% = 0 To m_maxLength - 1
                builder.Append(Convert.ToChar(randomize.Next(AscW("⠀"c), AscW("⠎"c))))
            Next

            If m_generatedNames.Contains(builder.ToString()) Then
                m_maxLength += 1
                Return Dots()
            End If

            m_generatedNames.Add(builder.ToString())
            Return builder.ToString()
        End Function

        Public Shared Function GenerateNewInvisible() As String
            Return Invisible()
        End Function

        Private Shared Function Invisible() As String
            Dim builder As New StringBuilder()
            Dim randomize As New Random(Guid.NewGuid().GetHashCode())

            For i% = 0 To m_maxLength - 1
                builder.Append(Convert.ToChar(randomize.Next(AscW("​"c), AscW("‏"c))))
            Next

            If m_generatedNames.Contains(builder.ToString()) Then
                m_maxLength += 1
                Return Invisible()
            End If

            m_generatedNames.Add(builder.ToString())
            Return builder.ToString()
        End Function

        ''' <summary>
        ''' by Yck from Confuser
        ''' </summary>
        Private Shared Function Chinese() As String
            Dim arr As New BitArray(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(Guid.NewGuid().GetHashCode().ToString())))

            Dim rand As New Random(Guid.NewGuid().GetHashCode())
            Dim xorB As Byte() = New Byte(arr.Length / 8 - 1) {}
            rand.NextBytes(xorB)
            Dim x As New BitArray(xorB)

            Dim result As BitArray = arr.Xor(x)
            Dim ret As Byte() = New Byte(result.Length / 8 - 1) {}
            result.CopyTo(ret, 0)

            Return Encoding.Unicode.GetString(ret).Replace(vbNullChar, "").Replace(".", "").Replace("/", "")
        End Function

        Private Shared Function Greek() As String
            Dim str$ = New String(Enumerable.Repeat(m_greek, m_rdn.Next(1, m_maxLength)).Select(Function(s) s(m_rdn.Next(s.Length))).ToArray())
            If m_generatedNames.Contains(str) Then
                m_maxLength += 1
                Return Greek()
            End If
            m_generatedNames.Add(str)
            Return str
        End Function

        Private Shared Function Symbols() As String
            Dim str$ = New String(Enumerable.Repeat(m_Symbols, m_rdn.Next(1, m_maxLength)).Select(Function(s) s(m_rdn.Next(s.Length))).ToArray())
            If m_generatedNames.Contains(str) Then
                m_maxLength += 1
                Return Symbols()
            End If
            m_generatedNames.Add(str)
            Return str
        End Function

        Private Shared Function Flowing() As String
            Dim str$ = New String(Enumerable.Repeat(m_flowing, m_rdn.Next(1, m_maxLength)).Select(Function(s) s(m_rdn.Next(s.Length))).ToArray())
            If m_generatedNames.Contains(str) Then
                m_maxLength += 1
                Return Flowing()
            End If
            m_generatedNames.Add(str)
            Return str
        End Function

        Public Shared Function GenerateNew() As String
            Select Case RandomizerType.RenameSetting
                Case RandomizerType.RenameEnum.Alphabetic
                    Return Alphabetic()
                Case RandomizerType.RenameEnum.Dot
                    Return Dots()
                Case RandomizerType.RenameEnum.Invisible
                    Return Invisible()
                Case RandomizerType.RenameEnum.Chinese
                    Return Chinese()
                Case RandomizerType.RenameEnum.Japanese
                    Return japanese()
                Case RandomizerType.RenameEnum.Greek
                    Return Greek()
                Case RandomizerType.RenameEnum.Symbols
                    Return Symbols()
                Case RandomizerType.RenameEnum.Flowing
                    Return Flowing()
            End Select

            Return Nothing
        End Function

        Public Shared Function GetScheme(IntVal%) As RenameEnum
            Select Case IntVal
                Case 0
                    Return RandomizerType.RenameEnum.Alphabetic
                Case 1
                    Return RandomizerType.RenameEnum.Dot
                Case 2
                    Return RandomizerType.RenameEnum.Invisible
                Case 3
                    Return RandomizerType.RenameEnum.Chinese
                Case 4
                    Return RandomizerType.RenameEnum.Japanese
                Case 5
                    Return RandomizerType.RenameEnum.Greek
                Case 6
                    Return RandomizerType.RenameEnum.Symbols
                Case 7
                    Return RandomizerType.RenameEnum.Flowing
            End Select

            Return RandomizerType.RenameEnum.Alphabetic
        End Function

        Public Shared Sub CleanUp()
            m_maxLength = 1
            m_generatedNames.Clear()
        End Sub
#End Region

    End Class
End Namespace
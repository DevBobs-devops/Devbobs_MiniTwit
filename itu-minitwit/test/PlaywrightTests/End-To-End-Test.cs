namespace PlaywrightTests;


using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public partial class EndToEnd : PageTest
{
    [Test]
    public async Task HasTitle()
    {
        await Page.GotoAsync("http://localhost:5221");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(MyRegex());
    }
    
    [Test]
    public async Task CanSeePublicTimeline()
    {
        await Page.GotoAsync("http://localhost:5221");
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Public Timeline" })).ToBeVisibleAsync();
    }
    
    [Test]
    public async Task HomePageHasRegisterButton()
    {
        await Page.GotoAsync("http://localhost:5221");
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Register" })).ToBeVisibleAsync();

    }
    
    [Test]
    public async Task HomePageHasLoginButton()
    {
        await Page.GotoAsync("http://localhost:5221");
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Login" })).ToBeVisibleAsync();
    }
    
    [Test]
    public async Task GoFromPublicTimelineToUserTimeLine()
    {
        await Page.GotoAsync("http://localhost:5221");
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Public Timeline" })).ToBeVisibleAsync();
        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine — 01-08-2023 13:17:39 Starbuck now is what we hear the" }).GetByRole(AriaRole.Link).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Jacqualine Gilcoine's Timeline" })).ToBeVisibleAsync();
    }
    
    
    [Test]
    public async Task GoFromUserTimeLineToPublicTimeline() 
    {
        await Page.GotoAsync("http://localhost:5221/Jacqualine%20Gilcoine");
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Jacqualine Gilcoine's Timeline" })).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Public Timeline" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Public Timeline" })).ToBeVisibleAsync();
    }
    
    [Test]
    public async Task JacqualineTimelineExists()
    {
        await Page.GotoAsync("http://localhost:5221/Jacqualine%20Gilcoine");
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Jacqualine Gilcoine's Timeline" })).ToBeVisibleAsync();
    }
    
    [Test]
    public async Task JacqualineCheepAboutStarbucksExists()
    {
        await Page.GotoAsync("http://localhost:5221/Jacqualine%20Gilcoine");
        await Page.GetByText("Starbuck now is what we hear").ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Starbuck now is what we hear the worst.");
    }
    
    [Test]
    public async Task CanClickRegisterPublicTimeLine()
    {
        await Page.GotoAsync("http://localhost:5221");
        
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Public Timeline" })).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Create a new account." })).ToBeVisibleAsync();
    }
    
    [Test]
    public async Task CanClickRegisterUserTimeLine()
    {
        await Page.GotoAsync("http://localhost:5221/Jacqualine%20Gilcoine");
        
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Jacqualine Gilcoine's Timeline" })).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Create a new account." })).ToBeVisibleAsync();
    }
    
    [Test]
    public async Task CanClickLoginPublicTimeline()
    {
        await Page.GotoAsync("http://localhost:5221");
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Public Timeline" })).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Use a local account to log in." })).ToBeVisibleAsync();
    }
    
    
    [Test]
    public async Task CanClickLoginUserTimeLine()
    {
        await Page.GotoAsync("http://localhost:5221/Jacqualine%20Gilcoine");
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Jacqualine Gilcoine's Timeline" })).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Use a local account to log in." })).ToBeVisibleAsync();
    }
    
    [Test]
    public async Task HasPage2()
    {
        await Page.GotoAsync("http://localhost:5221?Page=2");
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Public Timeline" })).ToBeVisibleAsync();
    }

    [Test] 
    public async Task CanSeeOtherUsersTimeline() 
    { 
        await Page.GotoAsync("http://localhost:5221/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Mellie Yost" }).ClickAsync();
        await Page.GetByText("But what was behind the").ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Mellie Yost's Timeline" }).ClickAsync();
    } 
    
    [Test]
    public async Task CanSeeRegisterPage()
    {
        await Page.GotoAsync("http://localhost:5221");

        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Register", Exact = true }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Create a new account." }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Use another service to" }).ClickAsync();
        await Page.GetByText("Username").ClickAsync();
        await Page.GetByText("Email").ClickAsync();
        await Page.GetByText("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByText("Confirm Password").ClickAsync();
    }
    
    // longer tests
        
        
    [Test]
    public async Task MakeTestAccount() // username = testUser    Password = Test123!    email: test@testmail.com
    {
        await Page.GotoAsync("http://localhost:5221/");
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register", Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@testmail.com");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Test123!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Test123!");
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "About me" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
        
    }

            
    [Test]
    public async Task CanCheep()
    {
        await Page.GotoAsync("http://localhost:5221/");
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register", Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@testmail.com");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Test123!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Test123!");
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Public Timeline" }).ClickAsync();
        
        // cheeps 
        await Page.Locator("#CheepMessage").ClickAsync();
        await Page.Locator("#CheepMessage").FillAsync("Hello Chirp!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Hello Chirp!");
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("testUser");
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Likes: 0");


        // deletes test user
        await Page.GetByRole(AriaRole.Link, new() { Name = "About me" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
        
    }
    
    
    [Test]
    public async Task ViewOtherUsersTimelineLoggedin()
    {
        await Page.GotoAsync("http://localhost:5221/");
        // logs in
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register", Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@testmail.com");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Test123!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Test123!");
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Public Timeline" }).ClickAsync();
        
        
        // clicks on Wendell Ballans username and shows their timeline
        await Page.GetByRole(AriaRole.Link, new() { Name = "Wendell Ballan" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Wendell Ballan's Timeline" })).ToBeVisibleAsync();
        await Page.GetByText("As I turned up one by one,").ClickAsync();

        // logs out 
        await Page.GetByRole(AriaRole.Link, new() { Name = "About me" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
    }


    [Test]
    public async Task LoginAndSeeOwnTimeline()
    {
        await Page.GotoAsync("http://localhost:5221/");
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register", Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@testmail.com");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Test123!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Test123!");
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Public Timeline" }).ClickAsync();
        
        // presses my timeline and confirms it is there
        await Page.GetByRole(AriaRole.Link, new() { Name = "My Timeline" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "testUser's Timeline" }).ClickAsync();

                
        await Page.GetByRole(AriaRole.Link, new() { Name = "About me" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
    }
    
    [Test]
    public async Task DeleteUserDeletesFollowedUsers()
    {
        await Page.GotoAsync("http://localhost:5221/");
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register", Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@testmail.com");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Test123!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();

        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Test123!");
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Public Timeline" }).ClickAsync();
        

        await Page.GetByRole(AriaRole.Link, new() { Name = "About me" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
        
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register", Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@testmail.com");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Test123!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();

        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Test123!");
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Public Timeline" }).ClickAsync();
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "About me" }).ClickAsync();
        await Expect(Page.Locator("body")).ToContainTextAsync("You're not following anyone.");
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "About me" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
    }
    
    
    [Test]
    public async Task CanFollowJacqualine()
    {
        await Page.GotoAsync("http://localhost:5221/");
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register", Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@testmail.com");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Test123!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Test123!");
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Public Timeline" }).ClickAsync();
        
        // asserts follow button is there and presses it 
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Follow");
        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine — 01-08-2023 13:17:36 The train pulled up at his" }).GetByRole(AriaRole.Button).First.ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Unfollow");

        // log out
        await Page.GetByRole(AriaRole.Link, new() { Name = "About me" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
    }
     
    [Test]
    public async Task CanFollowJacqualineAndUnfollow()
    {
        await Page.GotoAsync("http://localhost:5221/");
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register", Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@testmail.com");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Test123!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Test123!");
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Public Timeline" }).ClickAsync();
        
        // asserts follow button is there and presses it 
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Follow");
        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine — 01-08-2023 13:17:39 Starbuck now is what we hear the" }).GetByRole(AriaRole.Button).First.ClickAsync();

        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Unfollow");
        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine — 01-08-2023 13:17:39 Starbuck now is what we hear the" }).GetByRole(AriaRole.Button).First.ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Follow");
        
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "About me" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
    }

    [Test]
    public async Task CantLoginAfterDeleteData()
    {
        await Page.GotoAsync("http://localhost:5221/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("tester");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@test.dk");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("123Test!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("123Test!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Public Timeline" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("");
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("tester");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("123Test!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "About me" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").FillAsync("123Test!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("tester");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("123Test!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Listitem)).ToContainTextAsync("Invalid login attempt.");
    }


    [Test]
    public async Task CanLoginAndSeeLikeButton()
    {
        await Page.GotoAsync("http://localhost:5221/");
        // registers + logs in   
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register", Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@testmail.com");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Test123!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Test123!");
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Public Timeline" }).ClickAsync();
        
        // confirms like button is there 
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Like");
        
        // deletes account
        await Page.GetByRole(AriaRole.Link, new() { Name = "About me" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
        

    }


    [Test]
    public async Task CanLikeJacqualinesCheep()
    { 
        await Page.GotoAsync("http://localhost:5221/");
        // registers + logs in   
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register", Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@testmail.com");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Test123!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Test123!");
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Public Timeline" }).ClickAsync();
        
        // likes a cheep and confirms like goes up 
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Likes: 0");
        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine — 01-08-2023 13:17:39 Starbuck now is what we hear the" }).GetByRole(AriaRole.Button).Nth(1).ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Likes: 1");
        
        // deletes account
        await Page.GetByRole(AriaRole.Link, new() { Name = "About me" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
    }
    
    
    [Test]
    public async Task CanLikeJacqualinesCheepThenUnlike()
    {
        await Page.GotoAsync("http://localhost:5221/");
        // registers + logs in   
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register", Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@testmail.com");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Test123!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Test123!");
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Public Timeline" }).ClickAsync();
        
        // likes a cheep and confirms like count goes up  
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Likes: 0");
        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine — 01-08-2023 13:17:39 Starbuck now is what we hear the" }).GetByRole(AriaRole.Button).Nth(1).ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Likes: 1");
        
        // unlikes cheep and confirms it goes back to 0 likes
        await Page.GetByRole(AriaRole.Button, new() { Name = "Unlike" }).ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Likes: 0");
        
        // deletes account
        await Page.GetByRole(AriaRole.Link, new() { Name = "About me" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
    }
    
    [Test]
    public async Task CanLikeFromUserTimeline()
    { 
        await Page.GotoAsync("http://localhost:5221/");
        // registers + logs in   
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register", Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@testmail.com");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Test123!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Test123!");
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Public Timeline" }).ClickAsync();
        
        // Goes to Jacqualines Timeline
        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine — 01-08-2023 13:17:39 Starbuck now is what we hear the" }).GetByRole(AriaRole.Link).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Jacqualine Gilcoine's Timeline" })).ToBeVisibleAsync();
        
        // likes a cheep and confirms like count goes up 
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Likes: 0");
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Like");

        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine — 01-08-2023 13:17:39 Starbuck now is what we hear the" }).GetByRole(AriaRole.Button).Nth(1).ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Likes: 1");
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Unlike");
        
        // deletes account
        await Page.GetByRole(AriaRole.Link, new() { Name = "About me" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
        
    }
    
        [Test]
    public async Task CanLikeAndUnlikeFromUserTimeline()
    { 
        await Page.GotoAsync("http://localhost:5221/");
        // registers + logs in   
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register", Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@testmail.com");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Test123!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Test123!");
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Public Timeline" }).ClickAsync();
        
        // Goes to Jacqualines Timeline
        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine — 01-08-2023 13:17:39 Starbuck now is what we hear the" }).GetByRole(AriaRole.Link).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Jacqualine Gilcoine's Timeline" })).ToBeVisibleAsync();
        
        // likes a cheep and confirms like count goes up 
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Likes: 0");
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Like");

        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine — 01-08-2023 13:17:39 Starbuck now is what we hear the" }).GetByRole(AriaRole.Button).Nth(1).ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Likes: 1");
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Unlike");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Unlike" }).ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Likes: 0");
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Like");

        
        // deletes account
        await Page.GetByRole(AriaRole.Link, new() { Name = "About me" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
        
    }
    
    
    [Test]
    public async Task CanFollowThenLikeFromPersonalTimeline()
    {
        await Page.GotoAsync("http://localhost:5221/");
        // registers + logs in   
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register", Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@testmail.com");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Test123!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Test123!");
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Public Timeline" }).ClickAsync();
        
        // follows chirp account and goes to personal timeline
        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine — 01-08-2023 13:17:39 Starbuck now is what we hear the" }).GetByRole(AriaRole.Button).First.ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "My Timeline" }).ClickAsync();
        
        // likes a cheep and confirms like count goes up 
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Likes: 0");
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Like");
        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine — 01-08-2023 13:17:39 Starbuck now is what we hear the" }).GetByRole(AriaRole.Button).Nth(1).ClickAsync();

        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Likes: 1");
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Unlike");
        
        // deletes account
        await Page.GetByRole(AriaRole.Link, new() { Name = "About me" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
        
    }
    
    
        [Test]
    public async Task CanFollowThenLikeFromPersonalTimelineAndUnlike()
    {
        await Page.GotoAsync("http://localhost:5221/");
        // registers + logs in   
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register", Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@testmail.com");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Test123!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Test123!");
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Public Timeline" }).ClickAsync();
        
        // follows chirp account and goes to personal timeline
        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine — 01-08-2023 13:17:39 Starbuck now is what we hear the" }).GetByRole(AriaRole.Button).First.ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "My Timeline" }).ClickAsync();
        
        // likes a cheep and confirms like count goes up 
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Likes: 0");
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Like");
        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine — 01-08-2023 13:17:39 Starbuck now is what we hear the" }).GetByRole(AriaRole.Button).Nth(1).ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Likes: 1");
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Unlike");
        
        await Page.GetByRole(AriaRole.Button, new() { Name = "Unlike" }).ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Likes: 0");
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Like");
        
        // deletes account
        await Page.GetByRole(AriaRole.Link, new() { Name = "About me" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
        
    }
    
    
    [Test]
    public async Task CanGoFromPage1ToPage2()
    {
        await Page.GotoAsync("http://localhost:5221/");
        
        await Expect(Page.Locator("h3")).ToContainTextAsync("1");
        await Page.GetByRole(AriaRole.Link, new() { Name = ">" }).ClickAsync();
        await Expect(Page.Locator("h3")).ToContainTextAsync("2"); 
    }
    
    [Test]
    public async Task CanGoFromPage2ToPage1()
    {
        await Page.GotoAsync("http://localhost:5221?Page=2");

        await Expect(Page.Locator("h3")).ToContainTextAsync("2");
        await Page.GetByRole(AriaRole.Link, new() { Name = "<" }).ClickAsync();
        await Expect(Page.Locator("h3")).ToContainTextAsync("1");
        
    }
    
    
    [Test]
    public async Task CanGoFromPage1ToPage2AndBackLoggedIn()
    {
        await Page.GotoAsync("http://localhost:5221/");
     
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register", Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@testmail.com");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Test123!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Test123!");
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        
        
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Public Timeline" }).ClickAsync();
        await Expect(Page.Locator("body")).ToContainTextAsync("1");
        await Page.GetByRole(AriaRole.Link, new() { Name = ">" }).ClickAsync();
        await Expect(Page.Locator("body")).ToContainTextAsync("2");
        await Page.GetByRole(AriaRole.Link, new() { Name = "<" }).ClickAsync();
        await Expect(Page.Locator("body")).ToContainTextAsync("1");
        
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "About me" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
    }
    
    
        
    
    [Test]
    public async Task CanGoFromPage1ToPage2AndBackLoggedInUserTimeline()
    {
        await Page.GotoAsync("http://localhost:5221/");
     
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register", Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@testmail.com");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Test123!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Test123!");
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        
        
        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine — 01-08-2023 13:17:39 Starbuck now is what we hear the" }).GetByRole(AriaRole.Link).ClickAsync();
        await Expect(Page.Locator("body")).ToContainTextAsync("1");
        await Page.GetByRole(AriaRole.Link, new() { Name = ">" }).ClickAsync();
        await Expect(Page.Locator("body")).ToContainTextAsync("2");
        await Page.GetByRole(AriaRole.Link, new() { Name = "<" }).ClickAsync();
        await Expect(Page.Locator("body")).ToContainTextAsync("1");
        
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "About me" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
    }
    
    [Test]
    public async Task CanGoFromPage1ToPage2AndBackUserTimeline()
    {
        await Page.GotoAsync("http://localhost:5221/");
        
        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine — 01-08-2023 13:17:39 Starbuck now is what we hear the" }).GetByRole(AriaRole.Link).ClickAsync();
        await Expect(Page.Locator("body")).ToContainTextAsync("1");
        await Page.GetByRole(AriaRole.Link, new() { Name = ">" }).ClickAsync();
        await Expect(Page.Locator("body")).ToContainTextAsync("2");
        await Page.GetByRole(AriaRole.Link, new() { Name = "<" }).ClickAsync();
        await Expect(Page.Locator("body")).ToContainTextAsync("1");
        
    }
    
       [Test]
    public async Task CanGoFromPage1ToPage2AndBackPersonalTimeline()
    {
        await Page.GotoAsync("http://localhost:5221/");
     
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register", Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@testmail.com");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Test123!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Test123!");
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testUser");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Public Timeline" }).ClickAsync();
        await Page.Locator("li").Filter(new() { HasText = "Mellie Yost — 01-08-2023 13:" }).GetByRole(AriaRole.Button).First.ClickAsync();
        await Page.Locator("li").Filter(new() { HasText = "Quintin Sitts — 01-08-2023 13:17:32 It''s bad enough to appal the stoutest man" }).GetByRole(AriaRole.Button).First.ClickAsync();
        await Page.Locator("li").Filter(new() { HasText = "Malcolm Janski — 01-08-2023 13:17:29 At present I cannot spare energy and" }).GetByRole(AriaRole.Button).First.ClickAsync();
        await Page.Locator("li").Filter(new() { HasText = "Roger Histand — 01-08-2023 13:17:20 You can understand his regarding it as" }).GetByRole(AriaRole.Button).First.ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "My Timeline" }).ClickAsync();
        await Expect(Page.Locator("body")).ToContainTextAsync("1");
        await Page.GetByRole(AriaRole.Link, new() { Name = ">" }).ClickAsync();
        await Expect(Page.Locator("body")).ToContainTextAsync("2");
        await Page.GetByRole(AriaRole.Link, new() { Name = "<" }).ClickAsync();
        await Expect(Page.Locator("body")).ToContainTextAsync("1");
        
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "About me" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").FillAsync("Test123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
    }

    [Test]
    public async Task CanDeleteOwnCheep()
    {
        await Page.GotoAsync("http://localhost:5221/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("Pows");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("a@a");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Testkode0!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Testkode0!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("Pows");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Testkode0!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.Locator("#CheepMessage").ClickAsync();
        await Page.Locator("#CheepMessage").FillAsync("Cheeeep");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await Expect(Page.GetByText("Cheeeep")).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "About me" }).ClickAsync();
        await Expect(Page.GetByText("You have no cheeps so far.")).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").FillAsync("Testkode0!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
    }

    [GeneratedRegex("Chirp!")]
    private static partial Regex MyRegex();
}

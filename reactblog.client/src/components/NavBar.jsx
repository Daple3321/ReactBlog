export default function NavBar({
    username,
    onProfileClick,
    onDiscoverClick,
    onNewBlogClick,
    onLogoutClick,
}) {

    return (
        <header>
            <ul id="navbar">
                <li id="barElement"><button onClick={onProfileClick}>{username}</button></li>
                <li id="barElement"><button onClick={onDiscoverClick}>Discover</button></li>
                <li id="barElement"><button onClick={onNewBlogClick}>New blog</button></li>
                <li id="logoutElement"><button onClick={onLogoutClick}>Log out</button></li>
            </ul>
        </header>
    );
}

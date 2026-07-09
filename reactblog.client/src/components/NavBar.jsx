export default function NavBar({ onBlogsClick, onNewBlogClick }) {

    return (
        <header>
            <ul id="navbar">
                <li id="barElement"><button onClick={onBlogsClick}>Blogs</button></li>
                <li id="barElement"><button onClick={onNewBlogClick}>New blog</button></li>
            </ul>
        </header>
    );
}

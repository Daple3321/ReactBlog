import { useState } from 'react';
import {getBlog} from '../blogTools.jsx'

export default function EditBlogForm({ blogToEdit, onUpdated }) {
    const [blogCreated, setCreated] = useState(false);
    const openedBlog = blogToEdit;
    
    async function updateBlogRequest(blogForm) {

        const response = await fetch(`/blogs/${openedBlog.id}`, {
            method: "PUT",
            body: blogForm,
        });
        
        if(response.ok){
            const data = await response.json();
            console.log(`Blog updated! ID = ${data.id}`);
            onUpdated(data);
            setCreated(true);
        }  
    }
    
    if(blogCreated){
        
    }
    else{
        return (
            <div>
                <form action={f => updateBlogRequest(f)}>
                    <p>
                        <input name="name" type="text" placeholder='Blog title' defaultValue={openedBlog.name}/>
                    </p>
                    <p>
                        <textarea name="content" type="text" rows='15' cols='140' defaultValue={openedBlog.content}/>
                    </p>
                    <input type='submit' value="Update"/>
                </form>
            </div>
        );
    }
}

import { useState } from 'react';
import {getBlog} from '../blogTools.jsx'

export default function NewBlogForm({ onCreated }) {
    const [blogCreated, setCreated] = useState(false);
    let createdBlog = undefined;
    
    async function postRequest(formData) {
        
        console.log(formData);
        const response = await fetch('/blogs/new', {
            method: "POST",
            // headers: {
            //     "Content-type": "application/json; charset=UTF-8"
            // },
            body: formData,
            //formData: formData,
        });
        if(response.ok){
            const data = await response.json();
            console.log(`New blog created! ID = ${data.id}`);
            createdBlog = await getBlog(data.id);
            onCreated(createdBlog);
            setCreated(true);
        }  
    }
    
    if(blogCreated){
        
    }
    else{
        return (
            <div>
                <form action={f => postRequest(f)}>
                    <p><input name="name" type="text" placeholder='Blog title'/></p>
                    <p><textarea name="content" type="text" rows='15' cols='140'/></p>
                    <input type='submit' value="Create"/>
                </form>
            </div>
        );
    }
}

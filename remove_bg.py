from PIL import Image

def remove_white_bg(img_path, out_path):
    img = Image.open(img_path).convert("RGBA")
    data = img.getdata()
    
    new_data = []
    for item in data:
        # Check if the pixel is close to white
        if item[0] > 230 and item[1] > 230 and item[2] > 230:
            # Make it transparent
            new_data.append((255, 255, 255, 0))
        else:
            new_data.append(item)
            
    img.putdata(new_data)
    
    # Also crop the transparent borders
    bbox = img.getbbox()
    if bbox:
        img = img.crop(bbox)
        
    img.save(out_path, "PNG")

remove_white_bg("Cine-TPI/img/logo.jpg", "Cine-TPI/img/logo.png")

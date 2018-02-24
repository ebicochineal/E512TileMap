#! /usr/bin/env python3
import os
import sys
from PIL import Image, ImageDraw, ImageFont

S = """

 !"#$%&'()*+,-./
0123456789:;<=>?
@ABCDEFGHIJKLMNO
PQRSTUVWXYZ[\]^_
`abcdefghijklmno
pqrstuvwxyz{|}~


 ｡｢｣､･ｦｧｨｩｪｫｬｭｮｯ
ｰｱｲｳｴｵｶｷｸｹｺｻｼｽｾｿ
ﾀﾁﾂﾃﾄﾅﾆﾇﾈﾉﾊﾋﾌﾍﾎﾏ
ﾐﾑﾒﾓﾔﾕﾖﾗﾘﾙﾚﾛﾜﾝﾞﾟ


"""

# S = """

#  !"#$%&'()*+,-./
# 0123456789:;<=>?
# @ABCDEFGHIJKLMNO
# PQRSTUVWXYZ[\]^_
# `abcdefghijklmno
# pqrstuvwxyz{|}~
# 

#  ｡｢｣､･をぁぃぅぇぉゃゅょっ
# ーあいうえおかきくけこさしすせそ
# たちつてとなにぬねのはひふへほま
# みむめもやゆよらりるれろわんﾞﾟ


# """


class FontToImage:
    def __init__(self, path, size):
        self.w = size * 16
        self.h = size * 16
        self.fontsize = size
        self.fontpath = path
        self.cd = os.path.abspath(os.path.dirname(__file__))
        self.image = Image.new('P', (self.w, self.h))
        self.draw = ImageDraw.Draw(self.image)
        self.font = ImageFont.truetype(self.fontpath, self.fontsize-self.fontsize//4, encoding='unic')
        self.image.info['transparency'] = 0
        self.image.putpalette([0,0,0,0,0,0,255,255,255])
        # print(self.image.getpalette())
        print(path)
        print(size)
        # print(self.image)
        # print(self.image.info)
        # print(self.image.mode)
        
        self._create()
        
    def _create(self):
        self.draw.rectangle((0, 0, self.w, self.h), outline=0, fill=0)
        x = y = 0
        for i in S:
            if i == '\n':
                x = 0
                y += 1
            else:
                self.draw.text((self.fontsize * x+4, self.fontsize * y), i, font=self.font, fill=1)
                self.draw.text((self.fontsize * x, self.fontsize * y), i, font=self.font, fill=2)
                x += 1
        self.image.save(self.cd + '/' + 'font' + str(self.fontsize) + '.png')
        
if __name__ == '__main__':
    if len(sys.argv) > 1:
        FontToImage(sys.argv[1], min(max(int(sys.argv[2]), 8), 128))
        # ttf_font_path, fontsize_8-128

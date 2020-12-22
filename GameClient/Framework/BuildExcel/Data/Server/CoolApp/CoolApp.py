#-*-coding:utf-8-*-
import sys
reload(sys)
sys.setdefaultencoding('utf-8')
import wx

import os
import re

from wx.lib.mixins.listctrl import CheckListCtrlMixin, ListCtrlAutoWidthMixin
from collections import defaultdict

#PATH = unicode('your own path', 'utf-8')
PATH = unicode('E:/策划wc/resource/server_data', 'utf-8')

class CoolUI(wx.Frame):
        def __init__(self, parent, id, title):
                
                wx.Frame.__init__(self, parent, id, title, size=(1150, 800))
                self.InitUI()
                self.Centre() 
                self.Show()
                
        def InitUI(self):
                menubar = wx.MenuBar()
                
                fileMenu = wx.Menu()
                helpMenu = wx.Menu()
                menubar.Append(fileMenu, '&file')
                menubar.Append(helpMenu, '&help')
                
                exititem = fileMenu.Append(wx.ID_EXIT, 'Quit', 'Quit application')
                supportitem = helpMenu.Append(wx.ID_CLEAR, 'support', '111')
                self.SetMenuBar(menubar)
                self.Bind(wx.EVT_MENU, lambda e: self.Close(), id=wx.ID_EXIT)
                self.Bind(wx.EVT_MENU, self.ShowMessage, id=wx.ID_CLEAR)
                
                panel = wx.Panel(self, -1)

                vbox = wx.BoxSizer(wx.VERTICAL)
                vbox2 = wx.BoxSizer(wx.VERTICAL)
                hbox = wx.BoxSizer(wx.HORIZONTAL)

                leftPanel = wx.Panel(panel, -1)
                rightPanel = wx.Panel(panel, -1)
                
                self.log = wx.TextCtrl(rightPanel, -1, style=wx.TE_MULTILINE|wx.TE_RICH2)
                
                self.list = CheckListCtrl(rightPanel)
                self.list.InsertColumn(0, unicode('脚本名称', 'utf-8'), width=200)
                self.list.InsertColumn(1, unicode('内容', 'utf-8'))

                self.list.SetForegroundColour('#c56c00')

                self.core = CoolCore()
                files = self.core.getScipts()
                os.chdir(PATH)
                pattern = re.compile('^call')
                for filename in files:
                        index = self.list.InsertStringItem(sys.maxint, filename)
                        splitedLines = [line.strip().decode('gbk', 'utf-8').split() for line in open(filename, 'r') if pattern.match(line)]
                        dic = defaultdict(list)
                        for words in splitedLines:
                                dic[words[2]].append(words[3])
                                
                        str = ''
                        for key, val in dic.items():
                                str = str + '(' + key + ' : ['
                                for word in val:
                                        str = str + word + ', '
                                str = str[0:-2]
                                str = str + ']), '
                        str = str[0:-2]
                        str = str + '\n'
                        
                        self.list.SetStringItem(index, 1, str)
               

                sel = wx.Button(leftPanel, -1, unicode('全选', 'utf-8'), size=(80, -1))
                des = wx.Button(leftPanel, -1, unicode('清除选择', 'utf-8'), size=(80, -1))
                apl = wx.Button(leftPanel, -1, unicode('执行命令', 'utf-8'), size=(80, -1))
                quit_ = wx.Button(leftPanel, wx.ID_EXIT, '退出', size=(80, -1))
                
                

                #contrler_ = Contrl(self.list, self.log)
            
                self.Bind(wx.EVT_BUTTON, self.OnSelectAll, id=sel.GetId())
                self.Bind(wx.EVT_BUTTON, self.OnDeselectAll, id=des.GetId())
                self.Bind(wx.EVT_BUTTON, self.OnApply, id=apl.GetId())

                self.Bind(wx.EVT_BUTTON, lambda e: self.Close(), id=wx.ID_EXIT)
                

                vbox2.Add(sel, 0, wx.TOP, 5)
                vbox2.Add((-1, 5))
                vbox2.Add(des)
                vbox2.Add((-1, 5))
                vbox2.Add(apl)

                vbox2.Add((-1, 150))
                vbox2.Add(quit_)

                leftPanel.SetSizer(vbox2)

                vbox.Add(self.list, 5, wx.EXPAND | wx.TOP, 3)
                vbox.Add((-1, 10))
                vbox.Add(self.log, 2, wx.EXPAND)
                vbox.Add((-1, 10))

                rightPanel.SetSizer(vbox)

                hbox.Add(leftPanel, 0, wx.EXPAND | wx.RIGHT, 5)
                hbox.Add(rightPanel, 1, wx.EXPAND)
                hbox.Add((3, -1))

                panel.SetSizer(hbox)

        def OnSelectAll(self, event):
                num = self.list.GetItemCount()
                for i in range(num):
                        self.list.CheckItem(i)

        def OnDeselectAll(self, event):
                num = self.list.GetItemCount()
                for i in range(num):
                        self.list.CheckItem(i, False)

        def OnApply(self, event):
                num = self.list.GetItemCount()
                self.log.Clear()
                os.chdir(PATH)
                files=[]                
                for i in range(num):
                        if self.list.IsChecked(i):
                                filename=self.list.GetItemText(i)
                                files.append(filename)

                self.core.generateAll(files)
                logname = 'All.log'
                os.remove(logname)
                os.system('All.bat >>' + logname)
                map(lambda line : self.log.AppendText(line.strip().decode('gbk', 'utf-8')+'\n'), open(logname, 'r'))
                
        def ShowMessage(self):
                dial = wx.MessageBox('Download completed', 'Info', wx.ICON_INFORMATION)
                dial.ShowModal()
            
                                

class CoolCore:
        def __init__(self):
                self.path = PATH
                
        def getScipts(self):
                files = os.listdir(self.path)
                pattern = re.compile('.*bat$')
                filters = ( 'xlsc.bat', 'all.bat', 'All.bat')
                return [f for f in files if pattern.match(f) and f not in filters]

        def generateAll(self, files):
                f = file('All.bat', 'w')
                f.write(r"""del server_tmp\\xls_deploy_tool.log"""+'\n')
                
                [f.write('call  '+File+'\n') for File in files]
                
                f.write(r"""@echo off"""+'\n')
                f.write(r"""echo "All Convert Success" """+'\n')
                f.write(r"""@echo on"""+'\n')
                #f.write(r"""pause"""+'\n')


                
class CheckListCtrl(wx.ListCtrl, CheckListCtrlMixin, ListCtrlAutoWidthMixin):
    def __init__(self, parent):
        wx.ListCtrl.__init__(self, parent, -1, style=wx.LC_REPORT | wx.SUNKEN_BORDER)
        CheckListCtrlMixin.__init__(self)
        ListCtrlAutoWidthMixin.__init__(self)

def main ():
        app = wx.App()
        frame = CoolUI(None, -1, title='Cool UI')
        app.MainLoop()

if __name__ == '__main__':
        main()


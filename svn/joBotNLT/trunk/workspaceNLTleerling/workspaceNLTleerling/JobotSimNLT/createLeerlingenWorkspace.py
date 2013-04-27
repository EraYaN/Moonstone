import sys
import os
import shutil

import fileinput


defaultTargetDir='C:\Workspaces\workspaceNLTleerling'
if not os.path.isdir(r'C:\Workspaces'):
  defaultTargetDir=r'c:\workspaceNLTleerling'


removeDirectories=[
 # r'JobotSimNLT\bin\javaBot',
  
  r'JobotSimNLT\src\javaBot\JoBot',
  r'JobotSimNLT\src\javaBot\JPB2',
  r'JobotSimNLT\src\javaBot\Junior',

  r'JobotSimNLT\src\javaBot\Nano\Dance',
  r'JobotSimNLT\src\javaBot\Nano\Servo',
  r'JobotSimNLT\src\javaBot\Nano\Soccer',
  r'JobotSimNLT\src\javaBot\Nano\Test',
  r'JobotSimNLT\src\javaBot\Nano\Tester',

]

removeFiles=[

  r'JobotSimNLT\src\javaBot\Nano\Grid\CalibrateBehavior09.java',
  r'JobotSimNLT\src\javaBot\Nano\Grid\CuriousBehavior02.java',
  r'JobotSimNLT\src\javaBot\Nano\Grid\CuriousBehavior05.java',
  r'JobotSimNLT\src\javaBot\Nano\Grid\CuriousBehavior08.java',
  r'JobotSimNLT\src\javaBot\Nano\Grid\CuriousBehavior08C.java',
  r'JobotSimNLT\src\javaBot\Nano\Grid\CuriousBehavior08D.java',
  r'JobotSimNLT\src\javaBot\Nano\Grid\CuriousBehavior08F.java',
  r'JobotSimNLT\src\javaBot\Nano\Grid\CuriousBehavior08I.java',
  r'JobotSimNLT\src\javaBot\Nano\Grid\CuriousBehavior09.java',

  r'JobotSimNLT\src\javaBot\Nano\Rescue\CalibrateBehavior09.java',
  r'JobotSimNLT\src\javaBot\Nano\Rescue\CuriousBehavior05.java',
  r'JobotSimNLT\src\javaBot\Nano\Rescue\DriveBehavior04.java',
  r'JobotSimNLT\src\javaBot\Nano\Rescue\DriveBehavior05.java',
  r'JobotSimNLT\src\javaBot\Nano\Rescue\LineFollowerBehavior06.java',
  r'JobotSimNLT\src\javaBot\Nano\Rescue\LineFollowerBehavior07.java',
  r'JobotSimNLT\src\javaBot\Nano\Rescue\LineFollowerBehavior11.java',

]

changeFiles = [
  r'JobotSimNLT\src\javaBot\Simulator.java',
]

def getParentDir(dir):
  (head,tail)=os.path.split(dir)
  # paths ending with '/' only '/' is removed from head 
  # so for these paths we have to do the split again 
  if tail=='': 
    (head,tail)=os.path.split(head)
  return head

# copy source directory to an asked target directory  
def copyTree(src):  
    global defaultTargetDir  
    dest=''
    while not dest:
      msg="Give destination dir ( def= %s ) : " % defaultTargetDir
      dest=raw_input(msg)
      if not dest:
         dest=defaultTargetDir

      if os.path.isdir(dest):  
        print "directory '%s' already exist" % dest
        dest='' 
      else:  
        parentDir=getParentDir(dest)
        if not os.path.isdir(parentDir)  : 
           print "parent directory '%s' of  destination directory '%s' must already exist" % (parentDir,dest)
           dest='' 

        
        
    print 'copy "%s" to "%s"' % (src,dest)    
    
    #raw_input("press Enter to start copy tree")  
    shutil.copytree(src,dest)

    return dest


def normalizeFile(file):
   #print "normalizing path '%s' " % path
   return os.path.normpath(file) 

def normalizeDir(dir):
   #print "normalizing dir '%s' " % path
   return os.path.normpath(dir) + os.sep


def rmdir(dir):
    try : 
       shutil.rmtree(dir)
       print "removing dir '%s' : done" % dir
    except os.error, err : 
       print "removing dir '%s' : failed ; %s "  % (file,str(err)) 

# remove dirs within rootdir
def removeDirsAt(rootdir):
  global removeDirectories
  for path in removeDirectories:
    path=rootdir + os.sep + path
    path=normalizeDir(path)
    if os.path.isdir(path):
      rmdir(path)
    else :
      print "removing dir '%s' : not done because does not exist" % path


def rmfile(file):
    try : 
       os.remove(file)
       print "removing file '%s' : done" % file
    except os.error, err : 
       print "removing file '%s' : failed ; %s "  % (file,str(err)) 




# remove files within rootdir
def removeFilesAt(rootdir):
  global removeFiles
  for path in removeFiles:
    path=rootdir + os.sep + path
    path=normalizeFile(path)
    if os.path.isfile(path):
      rmfile(path)
    else :
      print "removing file '%s' : not done because does not exist" % path

# line with string :
#   - PY_COMMENT_ADD : gets commented with //
#   - PY_COMMENT_REMOVE : gets uncommented by removing // at begin of line   
def process(line):
      if line.find("PY_COMMENT") != -1 :
         if line.find("PY_COMMENT_ADD") != -1 :
           print "// " + line,
         else:
           #"PY_COMMENT_REMOVE";
           searchstr="//";
           startpos=line.find(searchstr) + len(searchstr)
           print line[startpos:],
      else:  
        print line,

def changeFilesAt(rootdir):
  global changeFiles
  files=[ normalizeFile(rootdir  + os.sep + file) for file in changeFiles ] 
  for file in files:
    #file= normalizeFile(rootdir  + os.sep + file)
    try:
      for line in fileinput.input([file],inplace=True,backup='.bak'): 
         process(line)
      print "changing file '%s' : done " % file  
    except Exception, err :
      print "changing file '%s' : failed ; %s "  % (file,str(err)) 
       
def getDirOfScript():	
  scriptdir=os.path.dirname(sys.argv[0])
  
  # get absolute path
  scriptdir=os.path.abspath(scriptdir)
  #same as : scriptdir=os.path.normpath(os.path.join(os.getcwd(), scriptdir))  see : http://docs.python.org/library/os.path.html

  scriptdir=normalizeDir(scriptdir)  
  
  return scriptdir
	   
def main():
  # get path where python script is locate (we assume this is at root of docenten workspace)
  src=getDirOfScript()
  # copy src directory to asked target
  dest=copyTree(src)
  dest=normalizeDir(dest)
  # remove specified removeDirs in dest
  removeDirsAt(dest)
  # remove specified removeFiles in dest
  removeFilesAt(dest) 
  # change files
  changeFilesAt(dest)
   
   
      
try:
  main()
  #file=r"c:\tmp\Simulator.java"
  #shutil.copy(file+".bak",file)
  #changeFile(file)
except Exception, e :
  print "Stopped by uncaught exception: " +  str(e)
  
raw_input("press Enter to end")   
    
   

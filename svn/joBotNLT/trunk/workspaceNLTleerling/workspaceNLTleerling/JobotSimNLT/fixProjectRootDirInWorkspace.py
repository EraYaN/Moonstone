import sys
import os
import shutil

import fileinput


# file where absolute paths of libraries are defined
classPathFile=r'JobotSimNLT\.classpath'  

# we search   in classPathFile for oldRootDir, to do this we look at absolute path of following lib in that path
# thus specify a relative path from the workspace rootdir for a library used in the classPathFile :
searchlib=r'JobotSimNLT/lib/junit.jar'
 
def normalizeFile(file):
   #print "normalizing path '%s' " % path
   return os.path.normpath(file) 

def normalizeDir(dir):
   #print "normalizing dir '%s' " % path
   return os.path.normpath(dir) + os.sep
   
   
def changeClassPathInProject(currentRootDir,newRootDir):
   global classPathFile

   # note: java uses / in classpath names instead of the windows backslash \
   newRootDir=convertToCygwinDir(newRootDir)   
 
   oldRootDir=getRootDirInClassfile(currentRootDir)
 
   file=normalizeFile(currentRootDir  + os.sep + classPathFile)
   try:
      for line in fileinput.input([file],inplace=True,backup='.bak'): 
         line=line.replace(oldRootDir, newRootDir);
         print line,
      print "changing classpath file '%s' : done " % file  
   except Exception, err :
      print "changing classpath file '%s' : failed ; %s "  % (file,str(err)) 



def convertToCygwinDir(dir):
    return dir.replace('\\',r'/')

	
def getNewRootDir(proposedDir):	
    msg="Give new root dir ( def= %s ) : " % proposedDir
    newRootDir=raw_input(msg)	
    if not newRootDir:
      newRootDir=proposedDir
    return newRootDir


def getRootDirInClassfile(rootDir):
   global classPathFile
   global searchlib
   
   file=os.path.normpath(os.path.join(rootDir,classPathFile))
   
   searchedline=''
   try:
      for line in fileinput.input([file]): 
          if line.find(searchlib) != -1 :
            searchedline=line 
            fileinput.close()
            break  
   except Exception, err :
      print "error reading file '%s' : failed ; %s "  % (file,str(err)) 
   
   leftstr=r'classpathentry kind="lib" path="'
   leftpos=searchedline.find(leftstr)+len(leftstr)
   rigthpos=searchedline.find(searchlib)
   foundRootDir=searchedline[leftpos:rigthpos]
   
   return foundRootDir


def getDirOfScript():	
  scriptdir=os.path.dirname(sys.argv[0])
  
  # get absolute path
  scriptdir=os.path.abspath(scriptdir)
  #same as : scriptdir=os.path.normpath(os.path.join(os.getcwd(), scriptdir))  see : http://docs.python.org/library/os.path.html

  scriptdir=normalizeDir(scriptdir)  
  
  return scriptdir
  
  
  
def main():
  
  
  # get path where python script is locate (we assume this is at root of  workspace)
  workspaceRootDir=getDirOfScript()
  
  newRootDir=getNewRootDir(workspaceRootDir)
  newRootDir=normalizeDir(newRootDir)
  
  #raise Exception(newRootDir)
  changeClassPathInProject(workspaceRootDir,newRootDir)
  
  
   

if __name__ == "__main__":

    try:
      main()
    except Exception, e :
      print "Stopped by uncaught exception: " +  str(e)
      
    raw_input("press Enter to end")   
         
   

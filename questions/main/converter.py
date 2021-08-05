import argparse 
import pandas 
from os import listdir
from os.path import isfile, join

if __name__ == "__main__":
   parser = argparse.ArgumentParser()
   parser.add_argument("-d", "--directory", type = str, default = "G:\Documents\Trivia", help = "The directory containing all the input files.")
   
   args = parser.parse_args()
   
   directory = args.directory 
   
   onlyfiles = [f for f in listdir(directory) if isfile(join(directory, f))]
   txt_only = [f for f in onlyfiles if f.endswith("txt")]
   questions = []
   
   for txt_file in txt_only:
      full_path = directory + "\\" + txt_file
      print("Processing file %s" % full_path)
      
      with open(full_path, 'r', encoding="utf-8") as f:
         lines = f.readlines() 
         
         for i in range(0, len(lines), 2):
            question = lines[i]        # We'll have to process this slightly more carefully.
            answer = lines[i + 1][9:] # Remove the `Answer:‌ ` from the beginning.
            
            question_start_idx = question.index("Question:")
            question = question[question_start_idx + 10:]
                        
            entry = {
               "subject": txt_file[:txt_file.index(".txt")],
               "grade": 5,
               "question": question.rstrip(), 
               "answer": answer.rstrip() 
            }
            
            questions.append(entry)
   
   df = pandas.DataFrame(questions)
   output_path = directory + "\\all_questions.csv"
   print("Writing results to file " + output_path)
   df.to_csv(output_path, sep = ";")
            
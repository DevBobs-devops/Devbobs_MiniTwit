import os

p = r"./diagrams/"

for e in os.scandir(p):
    if e.is_file():
        with open(e.path, "r") as input:
            out = open(r"./"+e.name+".tex","w")
            out.write("\\begin{plantuml}\n")

            for line in input:
                out.write(line)

            out.write("\\end{plantuml}")

print("texify done")

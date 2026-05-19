input = open("./diagrams/plantuml-test-file.puml","r")

out = open("./plantuml-test-file-puml.tex","w")
out.write("\\begin{plantuml}\n")

for line in input:
    out.write(line)

out.write("\\end{plantuml}")
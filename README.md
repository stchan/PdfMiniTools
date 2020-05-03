**THIS PROJECT IS ARCHIVED**
No further development or maintenance will be done. There are plenty of well maintained, and full featured tools that provide more functionality. I'd recommend having a look at either qpdf or PdfBox:

https://github.com/qpdf/qpdf
https://pdfbox.apache.org/


**PdfMiniTools** is a set of small command line  utilities for working with PDF files:
- pdfInfo - displays properties and form field content of a PDF file.
- pdfCat - joins two or more PDF files into a new file 
- pdfEvenOddMerge - creates a new PDF file by combining the contents of a PDF file containing only odd numbered pages with a PDF file containing only even numbered pages. (useful for combining output from single sided scanners).
- pdfExtract - extracts pages from a PDF file into a new file.
- pdfSplit - extracts a PDF file's contents into two or more new files.

**Installation**<br/>
Check the [releases](<https://github.com/stchan/PdfMiniTools/releases>) page for this project to download a prebuilt MSI package.

**Requirements**<br/>
Windows 7/2008R2 or later, and version 4.6.2 of the .Net framework is required.




**Usage (pdfInfo):**

        
        pdfInfo [-acfi] file
        
Options:


    -a, --all	 Show all available information (equivalent to -fi). 
    -c, --csv	 Display results as comma separated values (CSV). 
    -f, --fields Show form fields. 
    -i, --info	 Show basic PDF info. This is the default.

Example:

        
        pdfInfo file.pdf
        
Shows basic PDF info, such as page count, encryption, PDF version, etc. Equivalent to "pdfInfo -i file.pdf"



Example 2:


        pdfInfo -f file.pdf


Shows the PDF's form fields, and their values. PDF files that do not contain fill-in fields are unlikely to have any form fields to display.



Example 3:

        pdfInfo -ac file.pdf  

Will show basic PDF info, and any form fields, as comma separated values. Equivalent to "pdfInfo -cfi file.pdf"



**Usage (pdfCat):**


        pdfCat file1 file2 [file3...] outputfile


Example:
        pdfCat file1.pdf file2.pdf file3.pdf joinedfile.pdf  

Concatenates the contents of file1.pdf, file2.pdf, and file3.pdf into the new file joinedfile.pdf



**Usage (pdfEvenOddMerge):**


        pdfevenoddmerge [-s] oddfile evenfile outputfile

Example 1:

        pdfevenoddmerge file1.pdf file2.pdf combined.pdf

Creates the file *combined.pdf*, with odd pages from *file1.pdf*, and even pages from *file2.pdf*.



Example 2:

        pdfevenoddmerge -s file1.pdf file2.pdf combined.pdf

Same as Example 1, except any extra pages will be skipped. (ie if *file1.pdf* contains 10 pages, and *file2.pdf* contains 12,  pages 11 and 12 in *file2.pdf* will be ignored.



**Usage (pdfExtract):**

        pdfExtract -e {page1|startpage1-endpage1}[,{page2|startpage2-endpage2}] [-p prefix] inputfile

Options:

        -e, --extract Specifies the pages to extract. Required. 
        -p, --prefix  Set the output file(s) prefix. Default is the input filename. Optional.



Example:

        pdfExtract -e 12,16,23 inputfile.pdf

Extracts pages 12,16, and 23 from *inputfile.pdf.* Output filenames will be *inputfile_1.pdf*, *inputfile_2.pdf*, and *inputfile_3.pdf*.



Example 2:

        pdfExtract -e 10-16,23,31 inputfile.pdf

Extracts pages 10-16, 23, and 31 from *inputfile.pdf*. Output filenames will be *inputfile_1.pdf*, *inputfile2.pdf*, and *inputfile3.pdf*.



Example 3:
        pdfExtract -e 12 -p output inputfile.pdf  

Extracts page 12 from *inputfile.pdf*. Output filename will be *output1.pdf*.



**Usage (pdfSplit):**

        pdfSplit -{a|s} [page1,page2] [-p prefix] file

Options:
        -a, --allpages Split after every page. 
        -s, --splits   Pages to split at, separated by a comma. 
        -p, --prefix   Set the output files prefix. Default is the input filename. Optional.

Example:

        pdfSplit -s 11,21 -p splitfile file1.pdf  

Splits *file1.pdf* at pages 11, and 21 into three files:
*splitfile01.pdf* (contains pages 1-10) 
*splitfile02.pdf* (contains pages 11-20) 
*splitfile03.pdf* (contains all remaining pages)



Example 2:

         pdfSplit -a file.pdf   

Splits *file.pdf* at every page. Output files would be:
*file01.pdf* (page 1)
*file02.pdf* (page 2) 
*file03.pdf* (page 3) 
. 
. 
*fileXX.pdf* (page XX - last page)


**Acknowledgements**

The following third party components are used/included:
-   CommandLine Parser library (v1.8 stable) - [link][1]
-   iTextSharp (v4.1.6 - last LGPL version) - [link][2]
-   PDF icon from open icon library (LGPL ) - [link][3]
-   Installer graphics from Open Clip Art (public domain):<br/>
&nbsp; - [Amateur Astronomer][4]<br/>
&nbsp; - [Spyglass][5]  

[1]: <http://commandline.codeplex.com/>

[5]: <http://openclipart.org/detail/28059/spyglass1-by-crimperman>

[4]: <http://openclipart.org/detail/139579/amateur-astronomer-by-sunking2>

[3]: <http://openiconlibrary.sourceforge.net/gallery2/?./Icons/apps/acroread.png>

[2]: <http://itextsharp.svn.sourceforge.net/viewvc/itextsharp/tags/iTextSharp_4_1_6/>

iTextSharp 4.1.6 is used because of its license - later versions use the more restrictive AGPL.


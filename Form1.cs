using System;
using System.IO;
using System.Windows.Forms;

namespace FileExplorer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            PopulateTreeView();
        }

        private void PopulateTreeView()
        {
            // 시스템에 있는 논리 드라이브 목록을 가져옴
            string[] drives = Environment.GetLogicalDrives();

            // 각 드라이브에 대해 반복
            foreach (string drive in drives)
            {
                // 드라이브를 나타내는 새로운 노드 생성
                TreeNode driveNode = new TreeNode(drive);
                driveNode.Tag = drive;
                driveNode.ImageKey = "drive";
                driveNode.SelectedImageKey = "drive";
                treeView1.Nodes.Add(driveNode);// 트리뷰에 추가

                try
                {
                    // 드라이브의 루트 디렉터리에 있는 모든 하위 디렉터리 가져오기
                    string[] dirs = Directory.GetDirectories(drive);
                    foreach (string dir in dirs)
                    {
                        TreeNode dirNode = new TreeNode(Path.GetFileName(dir));
                        dirNode.Tag = dir;
                        dirNode.ImageKey = "folder";
                        dirNode.SelectedImageKey = "folder";
                        driveNode.Nodes.Add(dirNode);

                        try
                        {
                            string[] subdirs = Directory.GetDirectories(dir);
                            // 각 디렉터리에 대해 반복
                            foreach (string subdir in subdirs)
                            {
                                // 디렉터리를 나타내는 새로운 노드 생성
                                TreeNode subDirNode = new TreeNode(Path.GetFileName(subdir));
                                subDirNode.Tag = subdir;
                                subDirNode.ImageKey = "folder";
                                subDirNode.SelectedImageKey = "folder";
                                dirNode.Nodes.Add(subDirNode);// 드라이브 노드의 하위 노드로 추가
                            }
                        }
                        catch { }
                    }
                }
                catch { }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            listView1.Items.Clear();// 리스트뷰 초기화
            string[] files = Directory.GetFiles(e.Node.Tag.ToString());// 선택한 노드의 태그로부터 파일 목록을 가져옴

            // 파일 목록에 대해 반복
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                FileItem item = new FileItem();
                item.Name = fileInfo.Name;
                item.Size = fileInfo.Length;
                item.LastModified = fileInfo.LastWriteTime;
                ListViewItem listViewItem = new ListViewItem(item.Name);
                listViewItem.SubItems.Add(item.Size.ToString());
                listViewItem.SubItems.Add(item.LastModified.ToString());
                listViewItem.Tag = file; // Set the Tag property to the full path of the file
                listView1.Items.Add(listViewItem);
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string fileName = listView1.SelectedItems[0].Tag.ToString();
                System.Diagnostics.Process.Start(fileName); // Open the file using the default associated application
            }
        }
    }

    public class FileItem
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
    }

    
}

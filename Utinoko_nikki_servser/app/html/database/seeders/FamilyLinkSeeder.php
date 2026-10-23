<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\DB;

class FamilyLinkSeeder extends Seeder
{
    public function run(): void
    {
        DB::table('family_links')->insert([
            [
                'elder_user_id' => '1',
                'family_user_id' => '1',
                'relationship' => '息子'
            ],
        ]);
    }
}
